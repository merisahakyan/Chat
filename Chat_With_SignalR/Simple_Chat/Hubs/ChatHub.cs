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
        }
        public void Send(string name, string message)
        {
            manager.InsertMessage(name, "general", message);
            Clients.All.addMessage(name, message);
        }
        public void SendToGroup(string username, string message, string roomname)
        {
            manager.InsertMessage(username, roomname, message);
            Clients.All.addMessageToRoom(username, message, roomname);
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
            if (ActiveUsers.All(x => x.ConnectionId != id) && ActiveUsers.All(x => x.Name != username))
            {
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
            if (manager.IsValidRoomName(group))
            {
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
            manager.JoinGroup(username, roomname);
            condition = "join";
        }
        public void OutFromRoom(string username, string roomname)
        {
            manager.OutFromRoom(username, roomname);
            Clients.All.outFromRoom(username);
            Clients.All.onRoomOut(username, roomname);
        }
        public void OutButton(string username, string roomname)
        {
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
            if (manager.IsValidNameOrEmail(username, email) && username.Length < 20 && password.Length >= 6)
            {
                MailProvider.SendMail(email, tok);
                manager.InsertUser(username, email, password, tok, false);
                t = true;
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