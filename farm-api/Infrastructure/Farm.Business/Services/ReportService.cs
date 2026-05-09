using Farm.Business.Services.Interfaces;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.ViewModels.Report;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    /// <summary>
    /// Heavy aggregation queries hit the DbContext directly to avoid N+1 calls
    /// across multiple repositories.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly FarmDbContext _db;

        public ReportService(FarmDbContext db) { _db = db; }

        public async Task<DashboardDto> GetDashboard(Guid? farmId = null)
        {
            var animals = _db.Animals.AsNoTracking().AsQueryable();
            if (farmId.HasValue) animals = animals.Where(a => a.Cage.FarmId == farmId.Value);

            var dto = new DashboardDto
            {
                TotalFarms = farmId.HasValue ? 1 : await _db.Farms.CountAsync(),
                TotalCages = farmId.HasValue
                    ? await _db.Cages.CountAsync(c => c.FarmId == farmId.Value)
                    : await _db.Cages.CountAsync(),
                TotalAnimals = await animals.CountAsync(),
                HealthyAnimals = await animals.CountAsync(a => a.HealthStatus == HealthStatus.Healthy),
                SickAnimals = await animals.CountAsync(a => a.HealthStatus == HealthStatus.Sick || a.HealthStatus == HealthStatus.Weak),
            };

            var today = DateTime.UtcNow.Date;
            var schedules = _db.VaccineSchedules.AsNoTracking().Where(s => s.Status == VaccineStatus.Scheduled);
            if (farmId.HasValue) schedules = schedules.Where(s => s.Animal.Cage.FarmId == farmId.Value);
            dto.VaccineSchedulesUpcoming = await schedules.CountAsync(s => s.ScheduledDate >= today && s.ScheduledDate <= today.AddDays(7));
            dto.VaccineSchedulesOverdue = await schedules.CountAsync(s => s.ScheduledDate < today);

            var alerts = _db.Alerts.AsNoTracking().Where(a => !a.IsRead);
            if (farmId.HasValue) alerts = alerts.Where(a => a.FarmId == farmId.Value);
            dto.UnreadAlerts = await alerts.CountAsync();
            dto.CriticalAlerts = await alerts.CountAsync(a => a.Severity == AlertSeverity.Critical);

            // Feed low-stock count uses today's transactions across the system / farm
            var feedItemIds = await _db.FeedItems.Where(f => f.IsActive).Select(f => f.Id).ToListAsync();
            var lowCount = 0;
            foreach (var id in feedItemIds)
            {
                var txQry = _db.FeedTransactions.AsNoTracking().Where(t => t.FeedItemId == id);
                if (farmId.HasValue) txQry = txQry.Where(t => t.FarmId == farmId.Value);
                var sIn = await txQry.Where(t => t.Type == FeedTransactionType.In || t.Type == FeedTransactionType.Adjustment)
                                     .SumAsync(t => (float?)t.Quantity) ?? 0;
                var sOut = await txQry.Where(t => t.Type == FeedTransactionType.Out || t.Type == FeedTransactionType.Loss)
                                      .SumAsync(t => (float?)t.Quantity) ?? 0;
                var stock = sIn - sOut;
                var threshold = await _db.FeedItems.Where(f => f.Id == id).Select(f => f.LowStockThreshold).FirstOrDefaultAsync();
                if (stock <= threshold) lowCount++;
            }
            dto.FeedItemsLowStock = lowCount;

            // Animals by species
            dto.AnimalsBySpecies = await animals.GroupBy(a => a.Species)
                .Select(g => new SpeciesCountDto { Species = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            // Animals by health
            dto.AnimalsByHealth = await animals.GroupBy(a => a.HealthStatus)
                .Select(g => new HealthCountDto { HealthStatus = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            // Last 6 months average weight
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var growthQry = _db.GrowthLogs.AsNoTracking().Where(g => g.RecordedAt >= sixMonthsAgo);
            if (farmId.HasValue) growthQry = growthQry.Where(g => g.Animal.Cage.FarmId == farmId.Value);
            dto.AverageGrowthByMonth = await growthQry
                .GroupBy(g => new { g.RecordedAt.Year, g.RecordedAt.Month })
                .Select(g => new MonthlyGrowthDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    AverageWeight = g.Average(x => x.Weight),
                    LogsCount = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            return dto;
        }
    }
}
