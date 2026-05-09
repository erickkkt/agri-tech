using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Farm.Api.Hubs
{
    /// <summary>
    /// Concrete SignalR-backed publisher. Sends "AlertCreated" with the alert payload.
    /// </summary>
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<NotificationHub> _hub;

        public SignalRNotificationPublisher(IHubContext<NotificationHub> hub) { _hub = hub; }

        public Task PushAlertGlobal(Alert alert)
            => _hub.Clients.All.SendAsync("AlertCreated", alert);

        public Task PushAlertToFarm(Guid farmId, Alert alert)
            => _hub.Clients.Group(NotificationHub.GroupFarmPrefix + farmId)
                           .SendAsync("AlertCreated", alert);

        public Task PushAlertToUser(Guid userId, Alert alert)
            => _hub.Clients.Group(NotificationHub.GroupUserPrefix + userId)
                           .SendAsync("AlertCreated", alert);
    }
}
