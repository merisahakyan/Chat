namespace Simple_Chat.Hubs
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR;
    using Models;
    using System;
    using Providers;
    using Repo.Models;

    public class ChatHub : Hub
    {
        private static List<User> ActiveUsers = new List<User>();
        static string uname;
        static bool login = false;
        static string condition = "";
        Repo.DataManager manager = new Repo.DataManager();

        public void Starting(string username, string roomname)
        {
            var id = Context.ConnectionId;
            UserModel user = null;

            if (!ReferenceEquals(username, null) && username != "")
                user = manager.GetUserByName(username);
            if (condition == "join")
            {

                if (!manager.RoomContainsUser(username, roomname))
                {
                    Clients.AllExcept(id).onNewRoomConnect(id, username, roomname);
                    JoinGroup(username, roomname);
                }

                ActiveUsers.Where(p => p.Name == username).First().ConnectionId = id;
                var users = manager.GetUsersByRoom(roomname);
                Clients.Client(id).onRoomConnected(id, username, users);
                condition = "";
                var messages = manager.GetMessagesByRoomName(roomname);
                Clients.Client(id).showAllMessages(roomname, messages);
            }
            else
            if (ActiveUsers.Where(p => p.Name == username).Count() == 1 && uname != null && !ReferenceEquals(user, null) && condition == "")
            {
                if (user.active)
                {
                    ActiveUsers.Where(p => p.Name == uname).First().ConnectionId = id;

                    var jr = manager.GetRoomsByUser(username);
                    var rooms = manager.GetRooms(username);

                    Clients.Client(id).onConnected(id, username, ActiveUsers, jr, rooms);
                    if (login)
                    {
                        Clients.AllExcept(id).onNewUserConnected(id, username, roomname);
                        login = false;
                    }

                    var messages = manager.GetMessagesByRoomName("general");
                    Clients.Client(id).showAllMessages("general", messages);

                }

            }
            else if (!ReferenceEquals(user, null) && username != null)
            {
                if (user.active == false)
                {
                    Clients.Client(id).onLoginFail();
                    login = false;
                }
            }
            //else if (ActiveUsers.Where(p => p.Name == username).Count() == 1 && roomname == "general")
            //{
            //    ActiveUsers.Where(p => p.Name == uname).First().ConnectionId = id;
            //    var rooms = _context.Rooms;
            //    List<Room> nr = new List<Room>();
            //    foreach (var m in rooms)
            //    {
            //        if (m.RoomName == "general")
            //            continue;
            //        Room r = new Room(m);
            //        nr.Add(r);
            //    }
            //    Clients.Client(id).onConnected(id, username, ActiveUsers, nr);
            //}

        }
        public void Send(string name, string message)
        {

            //_context.Messages.Add(new Simple_Chat.Message()
            //{
            //    MessageText = message,
            //    User = _context.Users.FirstOrDefault(p => p.UserName == name),
            //    Room = _context.Rooms.FirstOrDefault(p => p.RoomName == "general")
            //});
            //_context.SaveChanges();
            manager.InsertMessage(name, "general", message);
            Clients.All.addMessage(name, message);
        }
        public void SendToGroup(string username, string message, string roomname)
        {
            //_context.Messages.Add(new Simple_Chat.Message()
            //{
            //    MessageText = message,
            //    User = _context.Users.FirstOrDefault(p => p.UserName == username),
            //    Room = _context.Rooms.FirstOrDefault(p => p.RoomName == roomname)
            //});
            //_context.SaveChanges();
            manager.InsertMessage(username, roomname, message);
            Clients.All.addMessageToRoom(username, message, roomname);
            //var usersfornotify = _context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users;
            var usersfornotify = manager.GetUsersByRoom(roomname);
            List<string> notify = new List<string>();
            foreach (var m in usersfornotify)
            {
                if (ActiveUsers.Contains(new User(m)))
                    notify.Add(ActiveUsers.FirstOrDefault(p => p.Name == (new User(m)).Name).ConnectionId);
            }
            Clients.Clients(notify).desktopNot(roomname, username, message);
        }

        public void Connect(string username, string password)
        {
            var id = Context.ConnectionId;
            uname = username;
            login = true;
            // if (ActiveUsers.All(x => x.ConnectionId != id) && ActiveUsers.All(x => x.Name != username) && _context.Users.Where(p => p.UserName == username).Count() > 0)
            if (ActiveUsers.All(x => x.ConnectionId != id) && ActiveUsers.All(x => x.Name != username))
            {
                //var user = _context.Users.Where(p => p.UserName == username).First();
                //if (user.Password == password && user.active)

                if (manager.Login(username, password) && manager.GetUserByName(username).active)
                {
                    ActiveUsers.Add(new User() { ConnectionId = id, Name = username, Password = password });
                    Clients.Caller.onLogin();
                }
                else
                {
                    Clients.Caller.onLoginFail();
                }
            }
            else
            {
                Clients.Caller.onLoginFail();
            }
        }
        public void Disconnect(string condition, string p)
        {
            var id = Context.ConnectionId;
            base.OnDisconnected(true);
            if (condition == "join")
            {
                ChatHub.condition = condition;
            }
        }
        public void Create(string group)
        {
            // if (_context.Rooms.Where(p => p.RoomName == group).Count() == 0)
            if (manager.IsValidRoomName(group))
            {
                //var id = Context.ConnectionId;
                //_context.Rooms.Add(new Simple_Chat.Room() { RoomName = group });
                //_context.SaveChanges();
                manager.InsertRoom(group);
                Clients.All.onNewGroupCreating(group);
            }
            else
            {
                Clients.Caller.failedRoomCreating();
            }
        }
        public void JoinGroup(string username, string roomname)
        {
            //Simple_Chat.User user = _context.Users.FirstOrDefault(p => p.UserName == username);
            //_context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users.Add(user);
            //_context.SaveChanges();
            manager.JoinGroup(username, roomname);
            condition = "join";
        }
        public void OutFromRoom(string username, string roomname)
        {
            //Simple_Chat.User user = _context.Users.FirstOrDefault(p => p.UserName == username);
            //_context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users.Remove(user);
            //_context.SaveChanges();
            manager.OutFromRoom(username, roomname);
            Clients.All.outFromRoom(username);
            Clients.All.onRoomOut(username, roomname);
        }
        public void OutButton(string username, string roomname)
        {
            //Simple_Chat.User user = _context.Users.FirstOrDefault(p => p.UserName == username);
            //_context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users.Remove(user);
            //_context.SaveChanges();
            manager.OutFromRoom(username, roomname);
            Clients.Caller.outRoomButton(roomname);
            Clients.All.onRoomOut(username, roomname);
        }
        public void ToGeneral(string username, string roomname)
        {
            Clients.Caller.ToGeneral(username);
        }
        public void SubmitRegistration(string username, string password, string email)
        {
            string tok = Guid.NewGuid().ToString();
            bool t;
            //if (_context.Users.Where(p => p.UserName == username).Count() == 0 && _context.Users.Where(p => p.eMail == email).Count() == 0 && username.Length < 20 && password.Length >= 6)
            if (manager.IsValidNameOrEmail(username, email) && username.Length < 20 && password.Length >= 6)
            {
                MailProvider.SendMail(email, tok);
                //Simple_Chat.User user = new Simple_Chat.User() { active = false, eMail = email, Password = password, UserName = username, token = tok };
                manager.InsertUser(username, email, password, tok, false);
                // _context.Users.Add(user);
                t = true;
                // _context.SaveChanges();
            }
            else
            {
                t = false;
            }
            Clients.Caller.onRegistration(t);
        }
        public void LogOut(string username)
        {
            ActiveUsers.Remove(ActiveUsers.FirstOrDefault(p => p.Name == username));
            Clients.Caller.onLogOut();
            Clients.All.onUserDisconnected(username);
        }
    }
}