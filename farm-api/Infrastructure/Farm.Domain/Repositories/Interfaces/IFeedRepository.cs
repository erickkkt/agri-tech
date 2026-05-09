using Farm.Domain.Entities;

namespace Farm.Domain.Repositories.Interfaces
{
    public interface IFeedItemRepository : IGenericRepository<FeedItem> { }

    public interface IFeedTransactionRepository : IGenericRepository<FeedTransaction> { }

    public interface IFeedConsumptionRepository : IGenericRepository<FeedConsumption> { }
}
