using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class DataManager
    {
        ChatDataEntities context = new ChatDataEntities();
        public void InsertUser(string UserName, string eMail, string Password, string token, bool active)
        {
            context.InsertIntoUsers(UserName, eMail, Password, token, active);
        }
        public void InsertMessage(Guid guid, string username, string roomname, string message)
        {
            context.InsertIntoMessages(guid, username, roomname, message, DateTime.Now);
        }
        public void EditMessage(Guid id, string newmessage)
        {
            context.EditMessage(id, newmessage, DateTime.Now);
        }

        public void InsertRoom(string roomname)
        {
            context.InsertIntoRooms(roomname);
        }
        public bool IsValidNameOrEmail(string username, string email)
        {
            return context.Database.SqlQuery<bool>("select dbo.IsValidNameOrEmail('" + username + "','" + email + "')").FirstOrDefault();
        }
        public void OutFromRoom(string username, string roomname)
        {
            context.OutFromRoom(username, roomname);
        }
        public void JoinGroup(string username, string roomname)
        {
            context.JoinGroup(username, roomname);
        }
        public bool IsValidRoomName(string roomname)
        {
            return context.Database.SqlQuery<bool>("select dbo.IsValidRoomName('" + roomname + "')").FirstOrDefault();
        }
        public bool Login(string username, string password)
        {
            return context.Database.SqlQuery<bool>("select dbo.Login('" + username + "','" + password + "')").FirstOrDefault();
        }
        public bool RoomContainsUser(string username, string roomname)
        {
            return context.Database.SqlQuery<bool>("select dbo.RoomContainsUser('" + username + "','" + roomname + "')").FirstOrDefault();
        }
        public User GetUserByName(string username)
        {
            return context.Database.SqlQuery<User>("select * from Users where UserName='" + username + "'").FirstOrDefault();
        }
        public Room GetRoomByName(string roomname)
        {
            return context.Database.SqlQuery<Room>("select * from Rooms where RoomName='" + roomname + "'").FirstOrDefault();
        }

        public User GetUserByID(int id)
        {
            return context.Database.SqlQuery<User>("select * from Users where UserID=" + id).FirstOrDefault();
        }
        public List<OldMessage> GetHistory(string id)
        {
            return context.OldMessages.Select(p => p).Where(p => p.ID.ToString() == id).ToList();
        }
        public List<User> GetUsersByRoom(string roomname)
        {
            List<User> users = new List<User>();
            int id = GetRoomByName(roomname).RoomID;
            var items = context.UsersRooms.Select(p => p).Where(p => p.RoomID == id).ToList();
            foreach (var item in items)
            {
                users.Add(GetUserByID(item.UserID));
            }
            return users;
        }

        public List<Message> GetMessagesByRoomName(string roomname)
        {
            List<Message> messages = new List<Message>();
            int id = GetRoomByName(roomname).RoomID;
            return context.Messages.Select(p => p).Where(p => p.RoomID == id).ToList();
        }
        public Message GetMessageByID(Guid id)
        {
            return context.Messages.FirstOrDefault(p => p.ID == id);
        }
        public List<Room> GetRooms()
        {
            return context.Rooms.ToList();
        }
        public Room GetRoomByID(int id)
        {
            return context.Database.SqlQuery<Room>("select * from Rooms where RoomID=" + id).FirstOrDefault();
        }
        public List<Room> GetRoomsByUser(string username)
        {
            List<Room> rooms = new List<Room>();
            int id = context.Users.Where(p => p.UserName == username).Select(p => p.UserID).FirstOrDefault();
            var items = context.UsersRooms.Select(p => p).Where(p => p.UserID == id);
            foreach (var item in items)
            {
                rooms.Add(GetRoomByID(item.RoomID));
            }
            return rooms;

        }
        public string CheckingActivation(string token)
        {
            if (context.Users.Where(p => p.token == token).Count() == 1 && context.Users.Where(p => p.token == token).First().active == false)
            {
                context.Users.Where(p => p.token == token).First().active = true;
                context.Users.Where(p => p.token == token).First().token = Guid.NewGuid().ToString();
                context.SaveChanges();
                return "You'r activated your account!";

            }
            else
                return "Something went wrong!";
        }
    }
}
