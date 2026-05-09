using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class VaccineService : IVaccineService
    {
        private readonly IVaccineRepository _vaccineRepo;
        private readonly IVaccineScheduleRepository _scheduleRepo;

        public VaccineService(IVaccineRepository vaccineRepo, IVaccineScheduleRepository scheduleRepo)
        {
            _vaccineRepo = vaccineRepo;
            _scheduleRepo = scheduleRepo;
        }

        public async Task<IReadOnlyCollection<Vaccine>> GetVaccines(string sortField, string sortDirection, int pageIndex, int pageSize)
        {
            var qry = _vaccineRepo.QueryAll().AsNoTracking();
            qry = (sortField, sortDirection?.ToLower()) switch
            {
                ("Name", "desc") => qry.OrderByDescending(v => v.Name),
                ("Name", _)      => qry.OrderBy(v => v.Name),
                _                => qry.OrderBy(v => v.Name),
            };
            return await qry.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<int> CountVaccines() => _vaccineRepo.CountTotalRecordsAsync();

        public Task<Vaccine> GetVaccine(Guid id) => _vaccineRepo.GetAsync(v => v.Id == id);

        public async Task<Guid> CreateVaccine(Vaccine vaccine)
        {
            vaccine.CreatedAt = DateTime.UtcNow;
            var created = await _vaccineRepo.CreateAsync(vaccine);
            return created?.Id ?? Guid.Empty;
        }

        public Task<Vaccine> UpdateVaccine(Vaccine vaccine) => _vaccineRepo.UpdateAsync(vaccine);

        public async Task<IReadOnlyCollection<VaccineSchedule>> GetSchedulesByAnimal(Guid animalId)
        {
            return await _scheduleRepo.QueryMany(s => s.AnimalId == animalId)
                .Include(s => s.Vaccine)
                .Include(s => s.Animal)
                .OrderByDescending(s => s.ScheduledDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<VaccineSchedule>> GetUpcomingSchedules(int daysAhead)
        {
            var today = DateTime.UtcNow.Date;
            var until = today.AddDays(daysAhead);
            return await _scheduleRepo
                .QueryMany(s => s.Status == VaccineStatus.Scheduled && s.ScheduledDate <= until)
                .Include(s => s.Animal)
                .Include(s => s.Vaccine)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Guid> CreateSchedule(VaccineSchedule schedule)
        {
            if (schedule.ScheduledDate.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("ScheduledDate must be today or later.");

            schedule.Status = VaccineStatus.Scheduled;
            schedule.CreatedAt = DateTime.UtcNow;
            var created = await _scheduleRepo.CreateAsync(schedule);
            return created?.Id ?? Guid.Empty;
        }

        public async Task<VaccineSchedule> AdministerSchedule(Guid scheduleId, DateTime administeredAt, string administeredBy, string notes)
        {
            var s = await _scheduleRepo.GetAsync(x => x.Id == scheduleId);
            if (s == null) throw new KeyNotFoundException("Vaccine schedule not found");

            s.AdministeredDate = administeredAt;
            s.AdministeredBy = administeredBy;
            s.Status = VaccineStatus.Administered;
            if (!string.IsNullOrWhiteSpace(notes)) s.Notes = notes;
            return await _scheduleRepo.UpdateAsync(s);
        }

        public Task<VaccineSchedule> GetSchedule(Guid scheduleId) => _scheduleRepo.GetAsync(s => s.Id == scheduleId);
    }
}
