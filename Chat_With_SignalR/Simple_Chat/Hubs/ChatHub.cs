namespace Simple_Chat.Hubs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR;
    using Models;
    using System;
    using System.Web.SessionState;
    using System.Web;
    using System.Collections.Concurrent;


    public class ChatHub : Hub
    {
        private static List<User> Users = new List<User>();
        private static List<Room> Rooms = new List<Room>();
        bool flag = true;
        static Dictionary<string, List<string>> sessions = new Dictionary<string, List<string>>();

        public void Starting()
        {
            if (sessions.ContainsKey("session1"))
                sessions["session1"].Add(Context.ConnectionId);
            else
                sessions.Add("session1", new List<string> { Context.ConnectionId });
        }
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        public void Connect(string username)
        {
            var id = Context.ConnectionId;
            if (Users.All(x => x.ConnectionId != id))
            {
                Users.Add(new User() { ConnectionId = id, Name = username });
                Clients.Caller.onConnected(id, username, Users, Rooms);
                Clients.Client(sessions["session1"].Last()).onConnected(sessions["session1"].Last(), username, Users, Rooms);
                Clients.AllExcept(sessions["session1"].Last()).onNewUserConnected(sessions["session1"].Last(), username);
            }
        }


        //public override Task OnDisconnected(bool stopCalled)
        //{


        //    if (flag == false)
        //    {
        //        User item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        //        if (!ReferenceEquals(item, null))
        //        {
        //            Users.Remove(item);
        //            var id = Context.ConnectionId;
        //            Clients.All.onUserDisconnected(id, item.Name);
        //        }
        //    }
        //    return base.OnDisconnected(stopCalled);
        //}
        public void Disconnect()
        {
            base.OnDisconnected(true);
            
        }

        public void Create(string group)
        {
            var id = Context.ConnectionId;
            var guid = Guid.NewGuid();
            Rooms.Add(new Room
            {
                RoomID = guid,
                RoomName = group
            });
            Clients.Caller.onCreating(Rooms);
            Clients.AllExcept(id).onNewGroupCreating(guid, group);
        }



        public void JoinGroup(string roomname)
        {

            //flag = false;
            //Rooms.Where(p => p.RoomName == roomname).First().Members.Add(new User
            //{
            //    ConnectionId = id,
            //    Name = Users.Where(p => p.ConnectionId == id).First().Name
            //});


            Groups.Add(Context.ConnectionId, roomname);
            Clients.Caller.onRoomConnected();
        }


    }
}