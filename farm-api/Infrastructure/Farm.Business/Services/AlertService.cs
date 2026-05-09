using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.Repositories.Interfaces;
using Farm.Domain.ViewModels.Alert;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _repo;
        private readonly INotificationPublisher _publisher;

        public AlertService(IAlertRepository repo, INotificationPublisher publisher)
        {
            _repo = repo;
            _publisher = publisher;
        }

        public async Task<IReadOnlyCollection<Alert>> GetAlerts(bool? unreadOnly, Guid? farmId, int pageIndex, int pageSize)
        {
            var qry = _repo.QueryAll().AsNoTracking().AsQueryable();
            if (unreadOnly == true) qry = qry.Where(a => !a.IsRead);
            if (farmId.HasValue) qry = qry.Where(a => a.FarmId == farmId.Value);
            return await qry.OrderByDescending(a => a.CreatedAt)
                .Skip(pageIndex * pageSize).Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAlerts(bool? unreadOnly, Guid? farmId)
        {
            var qry = _repo.QueryAll().AsQueryable();
            if (unreadOnly == true) qry = qry.Where(a => !a.IsRead);
            if (farmId.HasValue) qry = qry.Where(a => a.FarmId == farmId.Value);
            return await qry.CountAsync();
        }

        public async Task<AlertSummaryDto> GetSummary(Guid? farmId)
        {
            var qry = _repo.QueryAll().Where(a => !a.IsRead);
            if (farmId.HasValue) qry = qry.Where(a => a.FarmId == farmId.Value);
            var summary = new AlertSummaryDto
            {
                TotalUnread = await qry.CountAsync(),
                CriticalUnread = await qry.CountAsync(a => a.Severity == AlertSeverity.Critical),
                WarningUnread = await qry.CountAsync(a => a.Severity == AlertSeverity.Warning)
            };
            return summary;
        }

        public async Task<Alert> MarkRead(Guid alertId, Guid userId)
        {
            var alert = await _repo.GetAsync(a => a.Id == alertId);
            if (alert == null) return null;
            if (!alert.IsRead)
            {
                alert.IsRead = true;
                alert.ReadAt = DateTime.UtcNow;
                alert.ReadByUserId = userId;
                await _repo.UpdateAsync(alert);
            }
            return alert;
        }

        public async Task<int> MarkAllRead(Guid? farmId, Guid userId)
        {
            var qry = _repo.QueryAll().Where(a => !a.IsRead);
            if (farmId.HasValue) qry = qry.Where(a => a.FarmId == farmId.Value);
            var pending = await qry.ToListAsync();
            var now = DateTime.UtcNow;
            foreach (var a in pending)
            {
                a.IsRead = true;
                a.ReadAt = now;
                a.ReadByUserId = userId;
            }
            if (pending.Count > 0)
            {
                // Save through any one of them; updates already tracked
                foreach (var a in pending) await _repo.UpdateAsync(a);
            }
            return pending.Count;
        }

        public async Task<Alert> RaiseAlert(AlertType type, AlertSeverity severity, string title, string message,
            Guid? farmId = null, Guid? animalId = null, Guid? feedItemId = null,
            Guid? vaccineScheduleId = null, string payload = null)
        {
            var alert = new Alert
            {
                Type = type,
                Severity = severity,
                Title = title,
                Message = message,
                FarmId = farmId,
                AnimalId = animalId,
                FeedItemId = feedItemId,
                VaccineScheduleId = vaccineScheduleId,
                Payload = payload,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
            };
            await _repo.CreateAsync(alert);

            // Push to realtime listeners (best-effort; do not break the caller on failure)
            try
            {
                if (alert.FarmId.HasValue)
                    await _publisher.PushAlertToFarm(alert.FarmId.Value, alert);
                else
                    await _publisher.PushAlertGlobal(alert);
            }
            catch
            {
                // Logging is handled at the publisher / hub level
            }

            return alert;
        }
    }
}
