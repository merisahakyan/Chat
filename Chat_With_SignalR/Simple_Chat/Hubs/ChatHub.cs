namespace Simple_Chat.Hubs
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR;
    using System;
    using Providers;
    using BLL;
    using BLL.Models;
    using System.Threading.Tasks;

    public class ChatHub : BaseHub
    {
        public override Task Starting(string username, string roomname)
        {
            return base.Starting(username, roomname);
        }
        public override Task Send(string name, string message)
        {
            return base.Send(name, message);
        }
        public override Task SendToGroup(string username, string message, string roomname)
        {
            return base.SendToGroup(username, message, roomname);
        }
        public override Task EditMessage(string id, string newmessage)
        {
            return base.EditMessage(id, newmessage);
        }
        public override Task Connect(string username, string password)
        {
            return base.Connect(username, password);
        }
        public override Task Disconnect(string condition, string p)
        {
            return base.Disconnect(condition, p);
        }
        public override Task Create(string group)
        {
            return base.Create(group);
        }
        public override Task OutButton(string username, string roomname)
        {
            return base.OutButton(username, roomname);
        }
        public override Task OutFromRoom(string username, string roomname)
        {
            return base.OutFromRoom(username, roomname);
        }
        public override Task ToGeneral(string username, string roomname)
        {
            return base.ToGeneral(username, roomname);
        }
        public override Task GetHistory(string id)
        {
            return base.GetHistory(id);
        }
        public override Task SubmitRegistration(string username, string password, string email)
        {
            return base.SubmitRegistration(username, password, email);
        }
        public override Task LogOut(string username)
        {
            return base.LogOut(username);
        }
    }
}