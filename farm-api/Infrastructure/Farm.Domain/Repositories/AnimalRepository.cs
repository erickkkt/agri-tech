using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class AnimalRepository : GenericRepository<Animal, FarmDbContext>, IAnimalRepository
    {
        public AnimalRepository(FarmDbContext context) : base(context)
        {
        }
    }
}
