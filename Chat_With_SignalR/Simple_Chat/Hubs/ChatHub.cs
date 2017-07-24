namespace Simple_Chat.Hubs
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR;
    using Models;
    using System;
    using Providers;

    public class ChatHub : Hub
    {
        private static List<User> ActiveUsers = new List<User>();
        static string uname;
        static bool login = false;
        static string condition = "";
        ChatDataContext _context = new ChatDataContext();

        public void Starting(string username, string roomname)
        {
            var id = Context.ConnectionId;
            Simple_Chat.User user = null;

            if (!ReferenceEquals(username, null) && username != "")
                user = _context.Users.Where(p => p.UserName == username).First();
            if (condition == "join")
            {

                if (!_context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users.Contains(_context.Users.FirstOrDefault(p => p.UserName == username)))
                {
                    Clients.AllExcept(id).onNewRoomConnect(id, username, roomname);
                    JoinGroup(username, roomname);
                }

                ActiveUsers.Where(p => p.Name == username).First().ConnectionId = id;
                var users = _context.Rooms.Where(p => p.RoomName == roomname).First().Users.ToList();
                List<User> nu = new List<User>();
                foreach (var m in users)
                {
                    User u = new User(m);
                    nu.Add(u);
                }
                Clients.Client(id).onRoomConnected(id, username, nu);
                condition = "";
                var messages = _context.Messages.Where(p => p.Room.RoomName == roomname);
                List<Message> nm = new List<Message>();
                foreach (var m in messages)
                {
                    Message n = new Message(m);
                    nm.Add(n);
                }
                Clients.Client(id).showAllMessages(roomname, nm);
            }
            else
            if (ActiveUsers.Where(p => p.Name == username).Count() == 1 && uname != null && !ReferenceEquals(user, null) && condition == "")
            {
                if (user.active)
                {
                    ActiveUsers.Where(p => p.Name == uname).First().ConnectionId = id;

                    var jrooms = user.Rooms;
                    List<Room> jr = new List<Room>();
                    foreach (var m in jrooms)
                    {
                        Room r = new Room(m);
                        jr.Add(r);
                    }
                    var rooms = _context.Rooms;
                    List<Room> nr = new List<Room>();
                    foreach (var m in rooms)
                    {
                        if (m.RoomName == "general")
                            continue;
                        Room r = new Room(m);
                        if (!jr.Contains(r))
                            nr.Add(r);
                    }

                    Clients.Client(id).onConnected(id, username, ActiveUsers, jr, nr);
                    if (login)
                    {
                        Clients.AllExcept(id).onNewUserConnected(id, username, roomname);
                        login = false;
                    }

                    var messages = _context.Messages.Where(p => p.Room.RoomName == "general");
                    List<Message> nm = new List<Message>();
                    foreach (var m in messages)
                    {
                        Message n = new Message(m);
                        nm.Add(n);
                    }
                    Clients.Client(id).showAllMessages("general", nm);

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
            else if (ActiveUsers.Where(p => p.Name == username).Count() == 1 && roomname == "general")
            {
                ActiveUsers.Where(p => p.Name == uname).First().ConnectionId = id;
                var rooms = _context.Rooms;
                List<Room> nr = new List<Room>();
                foreach (var m in rooms)
                {
                    if (m.RoomName == "general")
                        continue;
                    Room r = new Room(m);
                    nr.Add(r);
                }
                Clients.Client(id).onConnected(id, username, ActiveUsers, nr);
            }

        }
        public void Send(string name, string message)
        {

            _context.Messages.Add(new Simple_Chat.Message()
            {
                MessageText = message,
                User = _context.Users.FirstOrDefault(p => p.UserName == name),
                Room = _context.Rooms.FirstOrDefault(p => p.RoomName == "general")
            });
            _context.SaveChanges();

            Clients.All.addMessage(name, message);
        }
        public void SendToGroup(string username, string message, string roomname)
        {
            _context.Messages.Add(new Simple_Chat.Message()
            {
                MessageText = message,
                User = _context.Users.FirstOrDefault(p => p.UserName == username),
                Room = _context.Rooms.FirstOrDefault(p => p.RoomName == roomname)
            });
            _context.SaveChanges();
            Clients.All.addMessageToRoom(username, message, roomname);
            var usersfornotify = _context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users;

            List<string> notify = new List<string>();
            foreach (var m in usersfornotify)
            {
                if (ActiveUsers.Contains(new User(m)) && m.UserName != username)
                    notify.Add(ActiveUsers.FirstOrDefault(p => p.Name == (new User(m)).Name).ConnectionId);
            }
            Clients.Clients(notify).desktopNot(roomname, username, message);
        }

        public void Connect(string username, string password)
        {
            var id = Context.ConnectionId;
            uname = username;
            login = true;
            if (ActiveUsers.All(x => x.ConnectionId != id) && ActiveUsers.All(x => x.Name != username) && _context.Users.Where(p => p.UserName == username).Count() > 0)
            {
                var user = _context.Users.Where(p => p.UserName == username).First();
                if (user.Password == password && user.active)
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
            if (_context.Rooms.Where(p => p.RoomName == group).Count() == 0)
            {
                var id = Context.ConnectionId;
                _context.Rooms.Add(new Simple_Chat.Room() { RoomName = group });
                _context.SaveChanges();

                Clients.All.onNewGroupCreating(group);
            }
            else
            {
                Clients.Caller.failedRoomCreating();
            }
        }
        public void JoinGroup(string username, string roomname)
        {
            Simple_Chat.User user = _context.Users.FirstOrDefault(p => p.UserName == username);
            _context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users.Add(user);
            _context.SaveChanges();
            condition = "join";
        }
        public void OutFromRoom(string username, string roomname)
        {
            Simple_Chat.User user = _context.Users.FirstOrDefault(p => p.UserName == username);
            _context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users.Remove(user);
            _context.SaveChanges();
            Clients.All.outFromRoom(username);
            Clients.All.onRoomOut(username, roomname);
        }
        public void OutButton(string username, string roomname)
        {
            Simple_Chat.User user = _context.Users.FirstOrDefault(p => p.UserName == username);
            _context.Rooms.FirstOrDefault(p => p.RoomName == roomname).Users.Remove(user);
            _context.SaveChanges();
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
            if (_context.Users.Where(p => p.UserName == username).Count() == 0 && _context.Users.Where(p => p.eMail == email).Count() == 0 && username.Length < 20 && password.Length >= 6)
            {
                MailProvider.SendMail(email, tok);
                Simple_Chat.User user = new Simple_Chat.User() { active = false, eMail = email, Password = password, UserName = username, token = tok };
                _context.Users.Add(user);
                t = true;
                _context.SaveChanges();
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