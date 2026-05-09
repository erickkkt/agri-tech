using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class VaccineRepository : GenericRepository<Vaccine, FarmDbContext>, IVaccineRepository
    {
        public VaccineRepository(FarmDbContext context) : base(context) { }
    }

    public class VaccineScheduleRepository : GenericRepository<VaccineSchedule, FarmDbContext>, IVaccineScheduleRepository
    {
        public VaccineScheduleRepository(FarmDbContext context) : base(context) { }
    }
}
