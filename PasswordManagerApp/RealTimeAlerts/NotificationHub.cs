using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.RealTimeAlerts
{
    public class NotificationHub : Hub
    {

        public NotificationHub()
        {
            
        }
        public Task SendNotificationToUser(string user, string message)
        {
            
            return Clients.User(user).SendAsync("ReceiveAlert", message);
        }
        public override Task OnConnectedAsync()
        {
            
           // return Clients.User(authUserId).SendAsync("ReceiveAlert", authUserId, message);
            return base.OnConnectedAsync();
        }




    }
}
