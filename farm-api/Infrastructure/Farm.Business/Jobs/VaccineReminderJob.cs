using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Farm.Business.Jobs
{
    /// <summary>
    /// Daily 06:00 UTC. Raises an Alert (and pushes via SignalR if a notifier is wired)
    /// for every VaccineSchedule that is due within the next 3 days or already overdue,
    /// and that has not already received a reminder.
    /// </summary>
    public class VaccineReminderJob : IVaccineReminderJob
    {
        private readonly IVaccineScheduleRepository _scheduleRepo;
        private readonly IAlertService _alertService;
        private readonly ILogger<VaccineReminderJob> _logger;

        public VaccineReminderJob(IVaccineScheduleRepository scheduleRepo,
                                  IAlertService alertService,
                                  ILogger<VaccineReminderJob> logger)
        {
            _scheduleRepo = scheduleRepo;
            _alertService = alertService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var today = DateTime.UtcNow.Date;
            var horizon = today.AddDays(3);

            var due = await _scheduleRepo.QueryAll()
                .Include(s => s.Animal).ThenInclude(a => a.Cage)
                .Include(s => s.Vaccine)
                .Where(s => s.Status == VaccineStatus.Scheduled
                            && s.ScheduledDate <= horizon
                            && !s.ReminderSent)
                .ToListAsync();

            _logger.LogInformation("VaccineReminderJob: {Count} schedules due/upcoming.", due.Count);

            foreach (var s in due)
            {
                var overdue = s.ScheduledDate.Date < today;
                var type = overdue ? AlertType.MissedVaccine : AlertType.VaccineReminder;
                var severity = overdue ? AlertSeverity.Critical : AlertSeverity.Warning;
                var title = overdue
                    ? $"Vaccine overdue: {s.Vaccine?.Name}"
                    : $"Vaccine due in {(s.ScheduledDate.Date - today).Days} day(s): {s.Vaccine?.Name}";
                var message = $"Animal {s.Animal?.Code} - {s.Animal?.Name} - scheduled {s.ScheduledDate:yyyy-MM-dd}.";

                await _alertService.RaiseAlert(type, severity, title, message,
                    farmId: s.Animal?.Cage?.FarmId,
                    animalId: s.AnimalId,
                    vaccineScheduleId: s.Id);

                s.ReminderSent = true;

                if (overdue) s.Status = VaccineStatus.Missed;
                await _scheduleRepo.UpdateAsync(s);
            }
        }
    }
}
