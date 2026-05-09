using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly FarmDbContext _db;

        public InvestmentService(FarmDbContext db) { _db = db; }

        public async Task<IReadOnlyCollection<InvestmentOffer>> GetOpenOffers(int pageIndex, int pageSize)
        {
            return await _db.InvestmentOffers.AsNoTracking()
                .Include(o => o.Animal)
                .Include(o => o.Farm)
                .Where(o => o.Status == InvestmentOfferStatus.Open)
                .OrderByDescending(o => o.CreatedAt)
                .Skip(pageIndex * pageSize).Take(pageSize)
                .ToListAsync();
        }

        public Task<InvestmentOffer> GetOfferById(Guid id)
            => _db.InvestmentOffers.Include(o => o.Animal).Include(o => o.Farm).FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Guid> CreateOffer(InvestmentOffer offer)
        {
            offer.AvailableShares = offer.TotalShares;
            offer.Status = InvestmentOfferStatus.Open;
            offer.CreatedAt = DateTime.UtcNow;
            _db.InvestmentOffers.Add(offer);
            await _db.SaveChangesAsync();
            return offer.Id;
        }

        public async Task<Guid> PlaceOrder(InvestmentOrder order)
        {
            // Use a transaction for atomicity (concurrent investors)
            using var tx = await _db.Database.BeginTransactionAsync();
            var offer = await _db.InvestmentOffers.FirstOrDefaultAsync(o => o.Id == order.OfferId);
            if (offer == null) throw new KeyNotFoundException("Offer not found");
            if (offer.Status != InvestmentOfferStatus.Open) throw new InvalidOperationException("Offer is not open");
            if (order.ShareQty <= 0) throw new InvalidOperationException("ShareQty must be positive");
            if (offer.AvailableShares < order.ShareQty)
                throw new InvalidOperationException($"Not enough available shares. Available: {offer.AvailableShares}, requested: {order.ShareQty}");

            order.TotalAmount = offer.PricePerShare * order.ShareQty;
            order.Status = InvestmentOrderStatus.Pending;
            order.CreatedAt = DateTime.UtcNow;
            offer.AvailableShares -= order.ShareQty;
            if (offer.AvailableShares == 0) offer.Status = InvestmentOfferStatus.Closed;

            _db.InvestmentOrders.Add(order);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return order.Id;
        }

        public async Task<IReadOnlyCollection<InvestmentOrder>> GetOrdersByInvestor(Guid investorUserId)
        {
            return await _db.InvestmentOrders.AsNoTracking()
                .Include(o => o.Offer).ThenInclude(o => o.Animal)
                .Where(o => o.InvestorUserId == investorUserId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<InvestmentOrder> ConfirmOrder(Guid orderId)
        {
            var order = await _db.InvestmentOrders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");
            order.Status = InvestmentOrderStatus.Confirmed;
            order.ConfirmedAt = DateTime.UtcNow;

            _db.ShareCertificates.Add(new ShareCertificate
            {
                OrderId = order.Id,
                CertificateNo = $"CRT-{DateTime.UtcNow:yyyyMMdd}-{order.Id:N}".Substring(0, 24),
                IssuedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Guid> RecordHarvest(HarvestEvent evt)
        {
            evt.CreatedAt = DateTime.UtcNow;
            _db.HarvestEvents.Add(evt);
            await _db.SaveChangesAsync();
            return evt.Id;
        }

        public async Task<int> DistributeProfit(Guid harvestEventId)
        {
            var harvest = await _db.HarvestEvents.FirstOrDefaultAsync(h => h.Id == harvestEventId);
            if (harvest == null) throw new KeyNotFoundException("HarvestEvent not found");
            if (harvest.OfferId == null) throw new InvalidOperationException("HarvestEvent is not linked to an offer");

            var offer = await _db.InvestmentOffers.FirstOrDefaultAsync(o => o.Id == harvest.OfferId);
            if (offer == null) throw new InvalidOperationException("Offer no longer exists");

            var orders = await _db.InvestmentOrders
                .Where(o => o.OfferId == offer.Id && o.Status == InvestmentOrderStatus.Confirmed)
                .ToListAsync();
            if (orders.Count == 0) return 0;

            var totalShares = orders.Sum(o => o.ShareQty);
            if (totalShares <= 0) return 0;

            var poolForInvestors = harvest.GrossRevenue * offer.ProfitRatio;

            var rows = 0;
            foreach (var order in orders)
            {
                var amount = poolForInvestors * order.ShareQty / totalShares;
                _db.ProfitDistributions.Add(new ProfitDistribution
                {
                    HarvestEventId = harvest.Id,
                    InvestorUserId = order.InvestorUserId,
                    OrderId = order.Id,
                    Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero),
                    CalculatedAt = DateTime.UtcNow,
                    Status = "Pending"
                });
                rows++;
            }
            offer.Status = InvestmentOfferStatus.Harvested;
            await _db.SaveChangesAsync();
            return rows;
        }

        public async Task<Guid> RecordAnimalUpdate(AnimalUpdate update)
        {
            update.RecordedAt = update.RecordedAt == default ? DateTime.UtcNow : update.RecordedAt;
            _db.AnimalUpdates.Add(update);
            await _db.SaveChangesAsync();
            return update.Id;
        }

        public async Task<IReadOnlyCollection<AnimalUpdate>> GetAnimalUpdates(Guid animalId, int take = 50)
        {
            return await _db.AnimalUpdates.AsNoTracking()
                .Where(u => u.AnimalId == animalId)
                .OrderByDescending(u => u.RecordedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}
