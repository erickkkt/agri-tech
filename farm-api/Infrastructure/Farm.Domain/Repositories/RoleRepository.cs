using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class RoleRepository : GenericRepository<Entities.Role, FarmDbContext>, IRoleRepository
    {
        public RoleRepository(FarmDbContext context) : base(context)
        {
        }
    }
}
