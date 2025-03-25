using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR.Chat.API
{
    public class ChatHub : Hub
    {
        private readonly ConnectedUsersService _connectedUsersService;

        public ChatHub(ConnectedUsersService connectedUsersService)
        {
            _connectedUsersService = connectedUsersService;
        }

        public override async Task OnConnectedAsync()
        {
            _connectedUsersService.AddUser(Context.ConnectionId);
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connectedUsersService.RemoveUser(Context.ConnectionId);
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

