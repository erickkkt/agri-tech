using Farm.Business.Services.Interfaces;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Farm.Business.Jobs
{
    /// <summary>
    /// Weekly. For animals where the most recent GrowthLog is older than 14 days
    /// OR weight has not increased in 14 days, raise an Info alert.
    /// </summary>
    public class StagnantGrowthJob : IStagnantGrowthJob
    {
        private readonly FarmDbContext _db;
        private readonly IAlertService _alertService;
        private readonly ILogger<StagnantGrowthJob> _logger;

        public StagnantGrowthJob(FarmDbContext db, IAlertService alertService, ILogger<StagnantGrowthJob> logger)
        {
            _db = db;
            _alertService = alertService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-14);
            var animals = await _db.Animals.Include(a => a.Cage).Where(a => a.IsActive).AsNoTracking().ToListAsync();
            var raised = 0;

            foreach (var a in animals)
            {
                var logs = await _db.GrowthLogs.AsNoTracking()
                    .Where(g => g.AnimalId == a.Id)
                    .OrderByDescending(g => g.RecordedAt)
                    .Take(5).ToListAsync();

                if (logs.Count == 0)
                {
                    await _alertService.RaiseAlert(AlertType.StagnantGrowth, AlertSeverity.Info,
                        title: $"No growth log for {a.Code}",
                        message: "No GrowthLog has been recorded yet.",
                        farmId: a.Cage?.FarmId, animalId: a.Id);
                    raised++;
                    continue;
                }

                var latest = logs[0];
                if (latest.RecordedAt < cutoff)
                {
                    await _alertService.RaiseAlert(AlertType.StagnantGrowth, AlertSeverity.Info,
                        title: $"Stale growth data for {a.Code}",
                        message: $"Last recorded {latest.RecordedAt:yyyy-MM-dd}.",
                        farmId: a.Cage?.FarmId, animalId: a.Id);
                    raised++;
                    continue;
                }

                var oldest = logs.LastOrDefault(l => l.RecordedAt < latest.RecordedAt.AddDays(-13));
                if (oldest != null && latest.Weight <= oldest.Weight)
                {
                    await _alertService.RaiseAlert(AlertType.StagnantGrowth, AlertSeverity.Info,
                        title: $"No weight gain for {a.Code}",
                        message: $"Weight {oldest.Weight} → {latest.Weight} over 14 days.",
                        farmId: a.Cage?.FarmId, animalId: a.Id);
                    raised++;
                }
            }
            _logger.LogInformation("StagnantGrowthJob: raised {Raised} alerts.", raised);
        }
    }
}
