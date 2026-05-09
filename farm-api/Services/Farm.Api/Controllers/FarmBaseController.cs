using Farm.Api.Extensions;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    public class FarmBaseController : ControllerBase
    {
        private readonly IUserService _userService;
        public FarmBaseController(IUserService userService)
        {
            _userService = userService;
        }

        protected Task<UserDto> AuthorizedUser
        {
            get
            {
                return _userService.GetUserInfoByAuthenticated(AuthenticatedUser);
            }
        }

        protected UserProfile AuthenticatedUser => HttpContext.GetUserProfile();
    }
}
