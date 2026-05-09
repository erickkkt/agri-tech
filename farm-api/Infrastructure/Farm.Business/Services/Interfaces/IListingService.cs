using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Listing;

namespace Farm.Business.Services.Interfaces
{
    public interface IListingService
    {
        Task<(IReadOnlyCollection<Listing> items, int total)> Search(ListingSearchDto query);
        Task<Listing> GetById(Guid id);
        Task<Guid> Create(Listing listing, IEnumerable<string> photoUrls);
        Task<Listing> Update(Listing listing, IEnumerable<string> photoUrls);
        Task<bool> Delete(Guid id);
    }
}
