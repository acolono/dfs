using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DigitalFailState.Web.Hubs
{

    public class AppHub : Hub
    {
        public string GetMessage() {
            return $"Hello {this.Context.ConnectionId}";
        }

        public override Task OnConnectedAsync() {
            Clients.All.SendCoreAsync("NewConnection", new object[]{Context.ConnectionId});
            return base.OnConnectedAsync();
        }
    }
}
