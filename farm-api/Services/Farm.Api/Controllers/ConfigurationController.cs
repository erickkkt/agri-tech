using Farm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// All authentication configuration 
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/configuration")]
    [Produces("application/json")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        /// <summary>
        /// Configuration Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// All Admin settings for Authentication and Authorization
        /// </summary>
        /// <returns>Configuration type</returns>
        [HttpGet("admin")]
        public IActionResult GetAdminConfiguration()
        {
            var configuration = new Configuration
            {
                ApiBaseUrl = _configuration["Admin:ApiBaseUrl"],
                ClientId = _configuration["IdentityServerAuthentication:ClientId"],
                IdentityServerAddress = _configuration["IdentityServerAuthentication:Authority"],
                RedirectUrl = _configuration["Admin:BaseUrl"],
                Scope = _configuration["IdentityServerAuthentication:Scope"],
                SilentRefreshUrl = _configuration["Admin:SilentRefreshUrl"],
                AdminUrl = _configuration["Admin:BaseUrl"],
            };

            return Ok(configuration);
        }

        /// <summary>
        /// List of Navigation Items
        /// </summary>
        /// <returns>List of Navigation Items</returns>
        [HttpGet("nav-items")]
        public async Task<ActionResult<IReadOnlyCollection<NavigationItem>>> GetNavigationItems()
        {
            var items = NavigationItem.GetAll();          
            return Ok(items);
        }

        private class Configuration
        {
            public string ApiBaseUrl { get; set; }

            public string ClientId { get; set; }

            public string IdentityServerAddress { get; set; }

            public string RedirectUrl { get; set; }

            public string Scope { get; set; }

            public string SilentRefreshUrl { get; set; }

            public string AdminUrl { get; set; }
        }
    }
}
