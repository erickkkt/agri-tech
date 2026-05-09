using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Repositories.Interfaces;
using Farm.Domain.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace Farm.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;

        public UserService(IUserRepository userRepository, IMemoryCache cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public async Task<Guid> CreateUser(User user)
        {
            var createdUser = await _userRepository.CreateAsync(user);

            if (createdUser != null)
                return createdUser.Id;

            return Guid.Empty;
        }

        public async Task<IReadOnlyCollection<User>> GetUsers(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            var query = _userRepository.QueryAll();

            if (sortDirection.ToLower().Equals("asc"))
            {
                switch (sortField)
                {
                    case "UserName":
                        return await query.OrderBy(p => p.UserName).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "EmailAddress":
                        return await query.OrderBy(p => p.EmailAddress).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderBy(p => p.UserName).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                };
            }
            else
            {
                switch (sortField)
                {
                    case "UserName":
                        return await query.OrderByDescending(p => p.UserName).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "EmailAddress":
                        return await query.OrderByDescending(p => p.EmailAddress).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderByDescending(p => p.UserName).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                };
            }
        }

        public async Task<int> CountTotalRecords()
        {
            return await _userRepository.CountTotalRecordsAsync();
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await _userRepository.GetAsync(x => x.Id == userId);
        }

        public async Task<User> UpdateUser(User user)
        {
            var result = await _userRepository.UpdateAsync(user);
            return result;
        }

        public async Task<UserDto> GetUserInfoByAuthenticated(UserProfile userProfile)
        {
            var key = userProfile.Id;
            UserDto userInfo = _cache.Get<UserDto>(key);
            if (userInfo != null)
            {
                return userInfo;
            }

            var user = await _userRepository.GetAsync(x=> x.Id == userProfile.Id, new List<Expression<Func<User, object>>>
                {
                    exp=>exp.Role
                });

            if (user == null)
            {
                //Try to insert this user into User table
                var newUser = new User()
                {
                    Id = userProfile.Id,
                    UserName = userProfile.Username,
                    EmailAddress = userProfile.Email,
                    ChangedByUserName = "System",
                    PhoneNumber = string.IsNullOrEmpty(userProfile.PhoneNumber) ? "09123456789" : userProfile.PhoneNumber,
                    ChangedAt = DateTime.UtcNow,
                    ChangedByUserId = Guid.Empty,
                    IsActive = true,
                    RoleId = new Guid("68DFFDA0-B650-44AC-A599-6CDBDFD641E4")
                };

                await _userRepository.CreateAsync(newUser);

                userInfo = new UserDto()
                {
                    Id = newUser.Id,
                    UserName = newUser.UserName,
                    EmailAddress = newUser.EmailAddress,
                    PhoneNumber = newUser.PhoneNumber,
                    RoleId = newUser.RoleId.Value,
                    Role = "HO.SYSADMIN"
                };
            }
            else
            {
                userInfo = new UserDto()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.RoleId.Value,
                    Role = user.Role.Name
                };
            }

            if (userInfo != null)
            {
                _cache.Remove(key);
                _cache.Set(key, userInfo, TimeSpan.FromMinutes(30));

                return userInfo;
            }

            return null;
        }

        public bool ClearCache(Guid userId)
        {
            var key = userId;
            _cache.Remove(key);
            return true;
        }
    }
}
