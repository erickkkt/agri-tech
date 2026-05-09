using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class GrowthLogService : IGrowthLogService
    {
        private readonly IGrowthLogRepository _repo;

        public GrowthLogService(IGrowthLogRepository repo) { _repo = repo; }

        public async Task<IReadOnlyCollection<GrowthLog>> GetByAnimal(Guid animalId, int take = 100)
        {
            return await _repo.QueryMany(g => g.AnimalId == animalId)
                .Include(g => g.Animal)
                .OrderByDescending(g => g.RecordedAt)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<GrowthLog> GetById(Guid id) => _repo.GetAsync(g => g.Id == id);

        public async Task<Guid> Create(GrowthLog log)
        {
            log.CreatedAt = DateTime.UtcNow;
            var created = await _repo.CreateAsync(log);
            return created?.Id ?? Guid.Empty;
        }

        public Task<GrowthLog> Update(GrowthLog log) => _repo.UpdateAsync(log);

        public async Task<bool> Delete(Guid id)
        {
            var existing = await _repo.GetAsync(g => g.Id == id);
            return existing != null && await _repo.DeleteAsync(existing);
        }
    }
}
