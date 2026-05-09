using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class UserRepository : GenericRepository<Entities.User, FarmDbContext>, IUserRepository
    {
        public UserRepository(FarmDbContext context) : base(context)
        {
        }
    }
}
