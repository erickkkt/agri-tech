using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Cage;

namespace Farm.Business.Services.Interfaces
{
    public interface ICageService
    {
        Task<Guid> CreateCage(Cage Cage);
        Task<Cage> UpdateCage(Cage Cage);
        Task<Cage> GetCage(Guid Cage);
        Task<int> CountTotalRecords();
        Task<IReadOnlyCollection<Cage>> GetCages(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10);
        Task<IReadOnlyCollection<Cage>> GetActiveCages(Guid farmId);
    }
}
