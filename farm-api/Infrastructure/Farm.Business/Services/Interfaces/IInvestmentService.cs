using Farm.Domain.Entities;

namespace Farm.Business.Services.Interfaces
{
    public interface IInvestmentService
    {
        Task<IReadOnlyCollection<InvestmentOffer>> GetOpenOffers(int pageIndex, int pageSize);
        Task<InvestmentOffer> GetOfferById(Guid id);
        Task<Guid> CreateOffer(InvestmentOffer offer);

        Task<Guid> PlaceOrder(InvestmentOrder order);
        Task<IReadOnlyCollection<InvestmentOrder>> GetOrdersByInvestor(Guid investorUserId);
        Task<InvestmentOrder> ConfirmOrder(Guid orderId);

        Task<Guid> RecordHarvest(HarvestEvent evt);

        /// <summary>
        /// Computes ProfitDistribution rows for a given HarvestEvent and persists them.
        /// Investors get GrossRevenue * Offer.ProfitRatio, split by share count.
        /// </summary>
        Task<int> DistributeProfit(Guid harvestEventId);

        Task<Guid> RecordAnimalUpdate(AnimalUpdate update);
        Task<IReadOnlyCollection<AnimalUpdate>> GetAnimalUpdates(Guid animalId, int take = 50);
    }
}
