using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class FeedItemRepository : GenericRepository<FeedItem, FarmDbContext>, IFeedItemRepository
    {
        public FeedItemRepository(FarmDbContext context) : base(context) { }
    }

    public class FeedTransactionRepository : GenericRepository<FeedTransaction, FarmDbContext>, IFeedTransactionRepository
    {
        public FeedTransactionRepository(FarmDbContext context) : base(context) { }
    }

    public class FeedConsumptionRepository : GenericRepository<FeedConsumption, FarmDbContext>, IFeedConsumptionRepository
    {
        public FeedConsumptionRepository(FarmDbContext context) : base(context) { }
    }
}
