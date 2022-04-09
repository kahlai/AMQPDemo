using Amqp;
using Amqp.Framing;
using Amqp.Types;
using Amqp.Listener;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AMQPDemo.Hubs
{
    public class ChatHub : Hub
    {
       
       public override Task OnConnectedAsync()
        {
            Util.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Util.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async void SendMessage(string user, string message)
        {   
            //Note: for purpose of demo, send to all browser
            await Clients.All.SendAsync("ReceiveMessage", "Sending to " +  user ,"\"" + message + "\" - " +Util.sendMessageToAMQ(message));
        }

    }
}