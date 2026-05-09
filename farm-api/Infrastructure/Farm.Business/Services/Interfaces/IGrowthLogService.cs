using Farm.Domain.Entities;

namespace Farm.Business.Services.Interfaces
{
    public interface IGrowthLogService
    {
        Task<IReadOnlyCollection<GrowthLog>> GetByAnimal(Guid animalId, int take = 100);
        Task<GrowthLog> GetById(Guid id);
        Task<Guid> Create(GrowthLog log);
        Task<GrowthLog> Update(GrowthLog log);
        Task<bool> Delete(Guid id);
    }
}
