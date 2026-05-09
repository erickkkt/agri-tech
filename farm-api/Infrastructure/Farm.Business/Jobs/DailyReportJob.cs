using Farm.Business.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Farm.Business.Jobs
{
    /// <summary>
    /// Computes the dashboard snapshot and stores it in IMemoryCache so the
    /// Reports endpoint can return data instantly during the day.
    /// </summary>
    public class DailyReportJob : IDailyReportJob
    {
        public const string CacheKey = "report:dashboard:global";

        private readonly IReportService _reportService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DailyReportJob> _logger;

        public DailyReportJob(IReportService reportService, IMemoryCache cache, ILogger<DailyReportJob> logger)
        {
            _reportService = reportService;
            _cache = cache;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var dashboard = await _reportService.GetDashboard();
            _cache.Set(CacheKey, dashboard, TimeSpan.FromHours(24));
            _logger.LogInformation("DailyReportJob: dashboard cached. Animals={A}, Alerts={X}.",
                dashboard.TotalAnimals, dashboard.UnreadAlerts);
        }
    }
}
