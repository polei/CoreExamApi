using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure.Hubs
{
    /// <summary>
    /// 每个用户作为一个组？
    /// </summary>
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        { 
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User?.FindFirst("uid")?.Value);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User?.FindFirst("uid")?.Value);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
