using DateApp.Extensions;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DateApp.SignalIR
{
    public class PresenceHub(PrecenseTracker precenseTracker):Hub 
    {
        public override async Task OnConnectedAsync()
        {
            await precenseTracker.UserConnected(GetUserId() ?? "", Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline",
                GetUserId());
            var currentUsers= await precenseTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await precenseTracker.UserDisconnected(GetUserId() ?? "", Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline",
                GetUserId());
            await base.OnDisconnectedAsync(exception);
        }
        private string GetUserId()
        {
            return Context.User?.GetUserId() ?? "";
        }
    }
}
