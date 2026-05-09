using Farm.Domain.Entities;

namespace Farm.Business.Services.Interfaces
{
    /// <summary>
    /// Push notification adapter. Implementation lives in Farm.Api (SignalR-backed)
    /// but the interface lives here so Hangfire jobs can depend on it.
    /// </summary>
    public interface INotificationPublisher
    {
        Task PushAlertToFarm(Guid farmId, Alert alert);
        Task PushAlertToUser(Guid userId, Alert alert);
        Task PushAlertGlobal(Alert alert);
    }
}
