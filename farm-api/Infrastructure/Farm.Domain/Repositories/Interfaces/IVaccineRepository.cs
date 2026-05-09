using Farm.Domain.Entities;

namespace Farm.Domain.Repositories.Interfaces
{
    public interface IVaccineRepository : IGenericRepository<Vaccine> { }

    public interface IVaccineScheduleRepository : IGenericRepository<VaccineSchedule> { }
}
