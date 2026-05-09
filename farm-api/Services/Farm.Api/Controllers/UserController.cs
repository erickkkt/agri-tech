using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// User Controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : FarmBaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        /// <summary>
        /// User Controller class
        /// </summary>
        public UserController(IUserService userService, IMapper mapper) : base(userService)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get user info
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            var currentUser = await AuthorizedUser;
            return Ok(currentUser);
        }


        [HttpGet("sign-out")]
        public async Task<ActionResult<UserDto>> SignOut()
        {
            var user = await AuthorizedUser;
            var result = _userService.ClearCache(user.Id);
            return Ok(result);
        }
    }
}
