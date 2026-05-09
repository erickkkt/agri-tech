using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class AlertRepository : GenericRepository<Alert, FarmDbContext>, IAlertRepository
    {
        public AlertRepository(FarmDbContext context) : base(context) { }
    }
}
