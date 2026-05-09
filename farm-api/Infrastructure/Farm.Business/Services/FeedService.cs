using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.Repositories.Interfaces;
using Farm.Domain.ViewModels.Feed;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class FeedService : IFeedService
    {
        private readonly IFeedItemRepository _itemRepo;
        private readonly IFeedTransactionRepository _txRepo;
        private readonly IFeedConsumptionRepository _consumeRepo;

        public FeedService(IFeedItemRepository itemRepo,
                           IFeedTransactionRepository txRepo,
                           IFeedConsumptionRepository consumeRepo)
        {
            _itemRepo = itemRepo;
            _txRepo = txRepo;
            _consumeRepo = consumeRepo;
        }

        public async Task<IReadOnlyCollection<FeedItem>> GetFeedItems(int pageIndex, int pageSize)
        {
            return await _itemRepo.QueryAll()
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .Skip(pageIndex * pageSize).Take(pageSize)
                .ToListAsync();
        }

        public Task<int> CountFeedItems() => _itemRepo.CountTotalRecordsAsync();
        public Task<FeedItem> GetFeedItem(Guid id) => _itemRepo.GetAsync(x => x.Id == id);

        public async Task<Guid> CreateFeedItem(FeedItem item)
        {
            item.CreatedAt = DateTime.UtcNow;
            var created = await _itemRepo.CreateAsync(item);
            return created?.Id ?? Guid.Empty;
        }

        public Task<FeedItem> UpdateFeedItem(FeedItem item) => _itemRepo.UpdateAsync(item);

        public async Task<Guid> CreateTransaction(FeedTransaction tx)
        {
            // For Out / Loss, ensure stock won't go negative
            if (tx.Type == FeedTransactionType.Out || tx.Type == FeedTransactionType.Loss)
            {
                var stock = await CalculateStock(tx.FarmId, tx.FeedItemId);
                if (stock < tx.Quantity)
                    throw new InvalidOperationException($"Insufficient stock. Available: {stock}, requested: {tx.Quantity}.");
            }

            tx.CreatedAt = DateTime.UtcNow;
            var created = await _txRepo.CreateAsync(tx);
            return created?.Id ?? Guid.Empty;
        }

        public async Task<IReadOnlyCollection<FeedTransaction>> GetTransactions(Guid? farmId, Guid? feedItemId, DateTime? from, DateTime? to)
        {
            var qry = _txRepo.QueryAll().Include(t => t.FeedItem).Include(t => t.Farm).AsNoTracking().AsQueryable();
            if (farmId.HasValue) qry = qry.Where(t => t.FarmId == farmId.Value);
            if (feedItemId.HasValue) qry = qry.Where(t => t.FeedItemId == feedItemId.Value);
            if (from.HasValue) qry = qry.Where(t => t.TransactionDate >= from.Value);
            if (to.HasValue) qry = qry.Where(t => t.TransactionDate <= to.Value);
            return await qry.OrderByDescending(t => t.TransactionDate).Take(500).ToListAsync();
        }

        public async Task<Guid> CreateConsumption(FeedConsumption consumption)
        {
            if (consumption.AnimalId == null && consumption.CageId == null)
                throw new InvalidOperationException("Consumption must reference an Animal or a Cage.");
            consumption.CreatedAt = DateTime.UtcNow;
            var created = await _consumeRepo.CreateAsync(consumption);
            return created?.Id ?? Guid.Empty;
        }

        public async Task<IReadOnlyCollection<FeedConsumption>> GetConsumptions(Guid? animalId, Guid? cageId, DateTime? from, DateTime? to)
        {
            var qry = _consumeRepo.QueryAll().Include(c => c.FeedItem).AsNoTracking().AsQueryable();
            if (animalId.HasValue) qry = qry.Where(c => c.AnimalId == animalId.Value);
            if (cageId.HasValue) qry = qry.Where(c => c.CageId == cageId.Value);
            if (from.HasValue) qry = qry.Where(c => c.ConsumedAt >= from.Value);
            if (to.HasValue) qry = qry.Where(c => c.ConsumedAt <= to.Value);
            return await qry.OrderByDescending(c => c.ConsumedAt).Take(500).ToListAsync();
        }

        public async Task<FeedSummaryDto> GetSummary(Guid farmId)
        {
            var items = await _itemRepo.QueryAll().Where(i => i.IsActive).AsNoTracking().ToListAsync();
            var dto = new FeedSummaryDto { FarmId = farmId };
            foreach (var i in items)
            {
                var (sIn, sOut) = await SumByDirection(farmId, i.Id);
                dto.Items.Add(new FeedStockLevelDto
                {
                    FeedItemId = i.Id,
                    FeedItemCode = i.Code,
                    FeedItemName = i.Name,
                    Unit = i.Unit,
                    TotalIn = sIn,
                    TotalOut = sOut,
                    Stock = sIn - sOut,
                    LowStockThreshold = i.LowStockThreshold,
                });
            }
            return dto;
        }

        public async Task<IReadOnlyCollection<FeedStockLevelDto>> GetLowStockItems()
        {
            // Aggregate across ALL farms for the system-wide job
            var items = await _itemRepo.QueryAll().Where(i => i.IsActive).AsNoTracking().ToListAsync();
            var result = new List<FeedStockLevelDto>();
            foreach (var i in items)
            {
                var totalIn = await _txRepo.QueryAll()
                    .Where(t => t.FeedItemId == i.Id &&
                                (t.Type == FeedTransactionType.In || t.Type == FeedTransactionType.Adjustment))
                    .SumAsync(t => (float?)t.Quantity) ?? 0;
                var totalOut = await _txRepo.QueryAll()
                    .Where(t => t.FeedItemId == i.Id &&
                                (t.Type == FeedTransactionType.Out || t.Type == FeedTransactionType.Loss))
                    .SumAsync(t => (float?)t.Quantity) ?? 0;
                var stock = totalIn - totalOut;
                if (stock <= i.LowStockThreshold)
                {
                    result.Add(new FeedStockLevelDto
                    {
                        FeedItemId = i.Id,
                        FeedItemCode = i.Code,
                        FeedItemName = i.Name,
                        Unit = i.Unit,
                        TotalIn = totalIn,
                        TotalOut = totalOut,
                        Stock = stock,
                        LowStockThreshold = i.LowStockThreshold,
                    });
                }
            }
            return result;
        }

        // ----------- helpers -----------
        private async Task<float> CalculateStock(Guid farmId, Guid feedItemId)
        {
            var (sIn, sOut) = await SumByDirection(farmId, feedItemId);
            return sIn - sOut;
        }

        private async Task<(float sIn, float sOut)> SumByDirection(Guid farmId, Guid feedItemId)
        {
            var qry = _txRepo.QueryAll().Where(t => t.FarmId == farmId && t.FeedItemId == feedItemId);
            var sIn = await qry.Where(t => t.Type == FeedTransactionType.In || t.Type == FeedTransactionType.Adjustment)
                               .SumAsync(t => (float?)t.Quantity) ?? 0;
            var sOut = await qry.Where(t => t.Type == FeedTransactionType.Out || t.Type == FeedTransactionType.Loss)
                                .SumAsync(t => (float?)t.Quantity) ?? 0;
            return (sIn, sOut);
        }
    }
}
