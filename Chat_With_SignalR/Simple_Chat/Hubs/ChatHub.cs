namespace Simple_Chat.Hubs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR;
    using Models;
    using System;

    public class ChatHub : Hub
    {
        private static List<User> Users = new List<User>();
        private static List<Room> Rooms = new List<Room>();
        bool flag = true;

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
                Clients.Caller.onConnected(id, username, Users);

                Clients.AllExcept(id).onNewUserConnected(id, username);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (flag)
            {
                User item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (!ReferenceEquals(item, null))
                {
                    Users.Remove(item);
                    var id = Context.ConnectionId;
                    Clients.All.onUserDisconnected(id, item.Name);
                }
            }
            return base.OnDisconnected(stopCalled);
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



        public void JoinGroup( string roomname)
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