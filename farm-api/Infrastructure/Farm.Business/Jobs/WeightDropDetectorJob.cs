using Farm.Business.Services.Interfaces;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Farm.Business.Jobs
{
    /// <summary>
    /// Daily 22:00 UTC. For every animal, compare the latest GrowthLog weight to
    /// the most recent log >= 7 days ago. If weight dropped >= 5%, raise an Alert.
    /// </summary>
    public class WeightDropDetectorJob : IWeightDropDetectorJob
    {
        private const float DROP_THRESHOLD = 0.05f; // 5%

        private readonly FarmDbContext _db;
        private readonly IAlertService _alertService;
        private readonly ILogger<WeightDropDetectorJob> _logger;

        public WeightDropDetectorJob(FarmDbContext db, IAlertService alertService, ILogger<WeightDropDetectorJob> logger)
        {
            _db = db;
            _alertService = alertService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var animalIds = await _db.GrowthLogs.Select(g => g.AnimalId).Distinct().ToListAsync();
            var alerts = 0;

            foreach (var animalId in animalIds)
            {
                var logs = await _db.GrowthLogs.AsNoTracking()
                    .Where(g => g.AnimalId == animalId)
                    .OrderByDescending(g => g.RecordedAt)
                    .Take(10)
                    .ToListAsync();
                if (logs.Count < 2) continue;

                var latest = logs[0];
                var weekAgoCutoff = latest.RecordedAt.AddDays(-7);
                var prior = logs.FirstOrDefault(l => l.RecordedAt <= weekAgoCutoff);
                if (prior == null) continue;
                if (prior.Weight <= 0) continue;

                var drop = (prior.Weight - latest.Weight) / prior.Weight;
                if (drop >= DROP_THRESHOLD)
                {
                    var animal = await _db.Animals
                        .Include(a => a.Cage)
                        .FirstOrDefaultAsync(a => a.Id == animalId);
                    if (animal == null) continue;

                    await _alertService.RaiseAlert(
                        AlertType.WeightDrop, AlertSeverity.Warning,
                        title: $"Animal {animal.Code} lost {drop:P1}",
                        message: $"Weight dropped from {prior.Weight} to {latest.Weight} between {prior.RecordedAt:yyyy-MM-dd} and {latest.RecordedAt:yyyy-MM-dd}.",
                        farmId: animal.Cage?.FarmId,
                        animalId: animal.Id);
                    alerts++;
                }
            }
            _logger.LogInformation("WeightDropDetectorJob: raised {Alerts} alerts.", alerts);
        }
    }
}
