using Farm.Business.Services.Interfaces;
using Farm.Domain.Enum;
using Microsoft.Extensions.Logging;

namespace Farm.Business.Jobs
{
    /// <summary>
    /// Every 6 hours. For each FeedItem whose total stock falls below threshold,
    /// raise an Alert with FeedItemId so the FE can deep-link to the inventory page.
    /// </summary>
    public class FeedLowStockJob : IFeedLowStockJob
    {
        private readonly IFeedService _feedService;
        private readonly IAlertService _alertService;
        private readonly ILogger<FeedLowStockJob> _logger;

        public FeedLowStockJob(IFeedService feedService, IAlertService alertService, ILogger<FeedLowStockJob> logger)
        {
            _feedService = feedService;
            _alertService = alertService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var lowStock = await _feedService.GetLowStockItems();
            _logger.LogInformation("FeedLowStockJob: {Count} low-stock feed items.", lowStock.Count);

            foreach (var i in lowStock)
            {
                await _alertService.RaiseAlert(
                    AlertType.FeedLowStock, AlertSeverity.Warning,
                    title: $"Low stock: {i.FeedItemName}",
                    message: $"Stock {i.Stock} {i.Unit} <= threshold {i.LowStockThreshold} {i.Unit}.",
                    feedItemId: i.FeedItemId,
                    payload: $"{{\"stock\":{i.Stock},\"threshold\":{i.LowStockThreshold}}}");
            }
        }
    }
}
