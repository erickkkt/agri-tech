using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class GrowthLogRepository : GenericRepository<GrowthLog, FarmDbContext>, IGrowthLogRepository
    {
        public GrowthLogRepository(FarmDbContext context) : base(context) { }
    }
}
