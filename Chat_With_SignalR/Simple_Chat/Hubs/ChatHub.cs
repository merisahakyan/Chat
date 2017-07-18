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
        static string uname;
        static bool login = false;
        static string condition;
        ChatDataContext _context = new ChatDataContext();

        public void Starting(string username, string roomname)
        {
            var id = Context.ConnectionId;
            if (Users.Where(p => p.Name == username).Count() == 1 && uname != null && login)
            {
                Users.Where(p => p.Name == uname).First().ConnectionId = id;
                Clients.Client(id).onConnected(id, username, Users, Rooms);
                Clients.AllExcept(id).onNewUserConnected(id, username);
                login = false;
            }
            else if (Users.Where(p => p.Name == username).Count() == 1 && roomname == "general")
            {
                Users.Where(p => p.Name == uname).First().ConnectionId = id;
                Clients.Client(id).onConnected(id, username, Users, Rooms);
            }
            if (condition == "join")
            {
                JoinGroup(username, roomname);
                Users.Where(p => p.Name == username).First().ConnectionId = id;
                Clients.Client(id).onRoomConnected(id, username, Rooms.Where(p => p.RoomName == roomname).First().Members);
                Clients.AllExcept(id).onNewRoomConnect(id, username, roomname);
                condition = "";
            }
            else if (Rooms.Where(p => p.Members.Where(o => o.Name == username).Count() > 0).Count() > 0)
            {

                Users.Where(p => p.Name == username).First().ConnectionId = id;
                Clients.Client(id).onRoomConnected(id, username, Rooms.Where(p => p.RoomName == roomname).First().Members);
            }
        }
        public void Send(string name, string message)
        {
            name = Users.Where(p => p.ConnectionId == Context.ConnectionId).First().Name;
            Clients.All.addMessage(name, message);
        }

        public void Connect(string id, string username)
        {
            id = Context.ConnectionId;
            uname = username;
            login = true;
            if (Users.All(x => x.ConnectionId != id))
            {
                Users.Add(new User() { ConnectionId = id, Name = username });
            }
        }


        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    User item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        //    if (!ReferenceEquals(item, null))
        //    {
        //        Users.Remove(item);
        //        var id = Context.ConnectionId;
        //        Clients.All.onUserDisconnected(id, item.Name);
        //    }

        //    return base.OnDisconnected(stopCalled);
        //}

        public void Disconnect(string condition, string p)
        {
            var id = Context.ConnectionId;
            base.OnDisconnected(true);
            if (condition == "join")
            {
                ChatHub.condition = condition;
            }
            else
            {

            }
        }

        public void Create(string group)
        {
            var id = Context.ConnectionId;
            var guid = Guid.NewGuid();
            Rooms.Add(new Room
            {
                RoomID = guid,
                RoomName = group,
                Members = new List<User>()
            });

            Clients.All.onNewGroupCreating(guid, group);

        }



        public void JoinGroup(string username, string roomname)
        {
            Rooms.Where(p => p.RoomName == roomname).First().Members.Add(new User
            {
                Name = username,
                ConnectionId = Context.ConnectionId
            });
            condition = "join";

        }
        public void OutFromRoom(string username, string roomname)
        {
            int index = -1;
            for (int i = 0; i < Rooms.Where(p => p.RoomName == roomname).First().Members.Count(); i++)
            {
                if (Rooms.Where(p => p.RoomName == roomname).First().Members[i].Name == username)
                {
                    index = i;
                    break;
                }
            }
            for (int i = 0; i < Rooms.Count(); i++)
            {
                if (Rooms[i].RoomName == roomname)
                {
                    Rooms[i].Members.RemoveRange(index, 1);
                    break;
                }
            }
            Clients.All.outFromRoom(username);
            Clients.All.onRoomOut(username, roomname);
        }

        public void SendToGroup(string username, string message, string roomname)
        {
            Clients.All.addMessageToRoom(username, message, roomname);
        }
    }
}