using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Feed;

namespace Farm.Business.Services.Interfaces
{
    public interface IFeedService
    {
        Task<IReadOnlyCollection<FeedItem>> GetFeedItems(int pageIndex, int pageSize);
        Task<int> CountFeedItems();
        Task<FeedItem> GetFeedItem(Guid id);
        Task<Guid> CreateFeedItem(FeedItem item);
        Task<FeedItem> UpdateFeedItem(FeedItem item);

        Task<Guid> CreateTransaction(FeedTransaction tx);
        Task<IReadOnlyCollection<FeedTransaction>> GetTransactions(Guid? farmId, Guid? feedItemId, DateTime? from, DateTime? to);

        Task<Guid> CreateConsumption(FeedConsumption consumption);
        Task<IReadOnlyCollection<FeedConsumption>> GetConsumptions(Guid? animalId, Guid? cageId, DateTime? from, DateTime? to);

        Task<FeedSummaryDto> GetSummary(Guid farmId);
        Task<IReadOnlyCollection<FeedStockLevelDto>> GetLowStockItems();
    }
}
