using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class FarmRepository : GenericRepository<Entities.Farm, FarmDbContext>, IFarmRepository
    {
        public FarmRepository(FarmDbContext context) : base(context)
        {
        }
    }
}
