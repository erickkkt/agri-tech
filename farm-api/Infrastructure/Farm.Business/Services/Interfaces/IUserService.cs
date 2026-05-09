using Farm.Domain.Entities;
using Farm.Domain.ViewModels.User;
namespace Farm.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<Guid> CreateUser(User User);
        Task<User> UpdateUser(User User);
        Task<User> GetUser(Guid User);
        Task<int> CountTotalRecords();
        Task<IReadOnlyCollection<User>> GetUsers(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10);

        Task<UserDto> GetUserInfoByAuthenticated(UserProfile userProfile);
        bool ClearCache(Guid userId);
    }
}
