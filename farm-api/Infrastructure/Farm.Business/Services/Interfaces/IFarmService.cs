

namespace Farm.Business.Services.Interfaces
{
    public interface IFarmService
    {
        Task<Guid> CreateFarm(Domain.Entities.Farm Farm);
        Task<Domain.Entities.Farm> UpdateFarm(Domain.Entities.Farm Farm);
        Task<Domain.Entities.Farm> GetFarm(Guid Farm);
        Task<int> CountTotalRecords();
        Task<IReadOnlyCollection<Domain.Entities.Farm>> GetFarms(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10);
        Task<IReadOnlyCollection<Domain.Entities.Farm>> GetActiveFarms();
    }
}
