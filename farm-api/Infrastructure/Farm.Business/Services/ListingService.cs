using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.ViewModels.Listing;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class ListingService : IListingService
    {
        private readonly FarmDbContext _db;

        public ListingService(FarmDbContext db) { _db = db; }

        public async Task<(IReadOnlyCollection<Listing> items, int total)> Search(ListingSearchDto q)
        {
            var qry = _db.Listings.AsNoTracking()
                .Include(l => l.Farm)
                .Include(l => l.Photos)
                .Where(l => l.Status == ListingStatus.Active)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Q))
                qry = qry.Where(l => l.Title.Contains(q.Q) || l.Description.Contains(q.Q));
            if (!string.IsNullOrWhiteSpace(q.Province))
                qry = qry.Where(l => l.Province == q.Province);
            if (q.Species.HasValue)
                qry = qry.Where(l => l.Species == q.Species.Value);
            if (q.Category.HasValue)
                qry = qry.Where(l => l.Category == q.Category.Value);

            var total = await qry.CountAsync();
            var items = await qry.OrderByDescending(l => l.CreatedAt)
                                 .Skip(q.PageIndex * q.PageSize)
                                 .Take(q.PageSize)
                                 .ToListAsync();
            return (items, total);
        }

        public Task<Listing> GetById(Guid id) => _db.Listings.Include(l => l.Photos).Include(l => l.Farm).FirstOrDefaultAsync(l => l.Id == id);

        public async Task<Guid> Create(Listing listing, IEnumerable<string> photoUrls)
        {
            listing.CreatedAt = DateTime.UtcNow;
            listing.Status = ListingStatus.Active;
            _db.Listings.Add(listing);

            int order = 0;
            foreach (var url in photoUrls ?? Enumerable.Empty<string>())
            {
                _db.ListingPhotos.Add(new ListingPhoto { ListingId = listing.Id, Url = url, Order = order++ });
            }
            await _db.SaveChangesAsync();
            return listing.Id;
        }

        public async Task<Listing> Update(Listing listing, IEnumerable<string> photoUrls)
        {
            _db.Listings.Update(listing);
            // Replace photos
            var existing = _db.ListingPhotos.Where(p => p.ListingId == listing.Id);
            _db.ListingPhotos.RemoveRange(existing);
            int order = 0;
            foreach (var url in photoUrls ?? Enumerable.Empty<string>())
            {
                _db.ListingPhotos.Add(new ListingPhoto { ListingId = listing.Id, Url = url, Order = order++ });
            }
            await _db.SaveChangesAsync();
            return listing;
        }

        public async Task<bool> Delete(Guid id)
        {
            var existing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == id);
            if (existing == null) return false;
            _db.Listings.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
