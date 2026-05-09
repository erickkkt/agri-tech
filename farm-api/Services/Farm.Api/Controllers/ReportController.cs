using Farm.Business.Jobs;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Farm.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reports")]
    [Produces("application/json")]
    [ApiController]
    public class ReportController : FarmBaseController
    {
        private readonly IReportService _reportService;
        private readonly IMemoryCache _cache;

        public ReportController(IReportService reportService, IMemoryCache cache, IUserService userService) : base(userService)
        {
            _reportService = reportService;
            _cache = cache;
        }

        /// <summary>
        /// Cached daily snapshot. Provide ?farmId= for farm-scoped dashboard (always live).
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> Dashboard([FromQuery] Guid? farmId)
        {
            if (!farmId.HasValue && _cache.TryGetValue<DashboardDto>(DailyReportJob.CacheKey, out var cached))
                return Ok(cached);

            var dto = await _reportService.GetDashboard(farmId);
            if (!farmId.HasValue) _cache.Set(DailyReportJob.CacheKey, dto, TimeSpan.FromHours(1));
            return Ok(dto);
        }
    }
}
