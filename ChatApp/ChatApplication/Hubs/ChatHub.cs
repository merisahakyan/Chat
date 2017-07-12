using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string name,string message)
        {
            Clients.All.addNewMessage(name, message);
        }
    }
}