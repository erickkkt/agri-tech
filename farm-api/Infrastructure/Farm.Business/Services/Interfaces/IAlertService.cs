using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.ViewModels.Alert;

namespace Farm.Business.Services.Interfaces
{
    public interface IAlertService
    {
        Task<IReadOnlyCollection<Alert>> GetAlerts(bool? unreadOnly, Guid? farmId, int pageIndex, int pageSize);
        Task<int> CountAlerts(bool? unreadOnly, Guid? farmId);
        Task<AlertSummaryDto> GetSummary(Guid? farmId);
        Task<Alert> MarkRead(Guid alertId, Guid userId);
        Task<int> MarkAllRead(Guid? farmId, Guid userId);

        /// <summary>
        /// Create + persist alert. Returns the created entity. Caller is responsible
        /// for pushing to SignalR (controller / job layer).
        /// </summary>
        Task<Alert> RaiseAlert(AlertType type, AlertSeverity severity, string title, string message,
            Guid? farmId = null, Guid? animalId = null, Guid? feedItemId = null,
            Guid? vaccineScheduleId = null, string payload = null);
    }
}
