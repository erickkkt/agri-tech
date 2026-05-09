using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{ 
    public class CageRepository : GenericRepository<Cage, FarmDbContext>, ICageRepository
    {
        public CageRepository(FarmDbContext context) : base(context)
        {
        }
    }
}
