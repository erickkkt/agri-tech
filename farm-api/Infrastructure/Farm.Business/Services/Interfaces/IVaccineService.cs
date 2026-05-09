using Farm.Domain.Entities;

namespace Farm.Business.Services.Interfaces
{
    public interface IVaccineService
    {
        Task<IReadOnlyCollection<Vaccine>> GetVaccines(string sortField, string sortDirection, int pageIndex, int pageSize);
        Task<int> CountVaccines();
        Task<Vaccine> GetVaccine(Guid id);
        Task<Guid> CreateVaccine(Vaccine vaccine);
        Task<Vaccine> UpdateVaccine(Vaccine vaccine);

        Task<IReadOnlyCollection<VaccineSchedule>> GetSchedulesByAnimal(Guid animalId);
        Task<IReadOnlyCollection<VaccineSchedule>> GetUpcomingSchedules(int daysAhead);
        Task<Guid> CreateSchedule(VaccineSchedule schedule);
        Task<VaccineSchedule> AdministerSchedule(Guid scheduleId, DateTime administeredAt, string administeredBy, string notes);
        Task<VaccineSchedule> GetSchedule(Guid scheduleId);
    }
}
