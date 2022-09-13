#nullable enable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CMS.Hubs
{
    [Authorize(Policy = "NotificationHubAuthorization")]
    public class NotificationHub : Hub
    {
        private ILogger<NotificationHub> _iLogger;
        public NotificationHub(ILogger<NotificationHub> iLogger)
        {
            this._iLogger = iLogger;
        }

        public Task SendMessageToAll(string eventTag, object message)
        {
            return Clients.All.SendAsync(eventTag, message);
        }

        public Task SendMessageToCaller(string eventTag, object message)
        {
            return Clients.Caller.SendAsync(eventTag, message);
        }

        public Task SendMessageToConnectionIdUser(string connectionId, string eventTag, object message)
        {
            return Clients.Client(connectionId).SendAsync(eventTag, message);
        }

        public Task SendMessageToUser(string eventTag, object message)
        {
            return Clients.Client(Context.ConnectionId).SendAsync(eventTag, message);
        }

        public Task JoinGroup(string group)
        {
            var feature = Context.Features.Get<IHttpContextFeature>();
            // this._iLogger.LogInformation($"JoinGroup {Context.ConnectionId} - {group} - {feature.HttpContext.User.Identity != null && feature.HttpContext.User.Identity.IsAuthenticated}");
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public Task SendMessageToGroup(string group, string eventTag, object message)
        {
            return Clients.Groups(group).SendAsync(eventTag, message);
        }

        public override async Task OnConnectedAsync()
        {
            // this._iLogger.LogInformation($"OnConnectedAsync {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // this._iLogger.LogInformation($"OnDisconnectedAsync {Context.ConnectionId}");
            // await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
