using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Farm.Api.Hubs
{
    /// <summary>
    /// Realtime notification hub. Clients call JoinFarmGroup(farmId) after auth
    /// to receive farm-scoped alerts. Personal alerts are pushed by user-id group
    /// "user:{userId}" (joined automatically on connect).
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
        public const string GroupFarmPrefix = "farm:";
        public const string GroupUserPrefix = "user:";

        public override async Task OnConnectedAsync()
        {
            // Auto-join personal group based on JWT subject claim
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupUserPrefix + userId);
            }
            await base.OnConnectedAsync();
        }

        public Task JoinFarmGroup(Guid farmId)
            => Groups.AddToGroupAsync(Context.ConnectionId, GroupFarmPrefix + farmId);

        public Task LeaveFarmGroup(Guid farmId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupFarmPrefix + farmId);
    }
}
