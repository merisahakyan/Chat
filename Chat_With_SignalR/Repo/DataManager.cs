using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Repo
{
    public class DataManager:IDisposable
    {
        ChatDataEntities context;
        public DataManager()
        {
            context = new ChatDataEntities();
        }

        public void InsertUser(string UserName, string eMail, string Password, string token, bool active)
        {
            context.sp_InsertIntoUsers(UserName, eMail, Password.GetHashCode().ToString(), token, active);
        }
        public void InsertMessage(Guid guid, string username, string roomname, string message)
        {
            context.sp_InsertIntoMessages(guid, username, roomname, message, DateTime.Now);
        }
        public void EditMessage(Guid id, string newmessage)
        {
            context.sp_EditMessage(id, newmessage, DateTime.Now);
        }

        public void InsertRoom(string roomname)
        {
            context.sp_InsertIntoRooms(roomname);
        }
        public bool IsValidNameOrEmail(string username, string email)
        {
            return context.Database.SqlQuery<bool>("select dbo.f_IsValidNameOrEmail('" + username + "','" + email + "')").FirstOrDefault();
        }
        public void OutFromRoom(string username, string roomname)
        {
            context.sp_OutFromRoom(username, roomname);
        }
        public void JoinGroup(string username, string roomname)
        {
            context.sp_JoinGroup(username, roomname);
        }
        public bool IsValidRoomName(string roomname)
        {
            return context.Database.SqlQuery<bool>("select dbo.f_IsValidRoomName('" + roomname + "')").FirstOrDefault();
        }
        public bool Login(string username, string password)
        {
            return context.Database.SqlQuery<bool>("select dbo.f_Login('" + username + "','" + password + "')").FirstOrDefault();
        }
        public bool RoomContainsUser(string username, string roomname)
        {
            return context.Database.SqlQuery<bool>("select dbo.f_RoomContainsUser('" + username + "','" + roomname + "')").FirstOrDefault();
        }
        public User GetUserByName(string username)
        {
            return context.Users.FirstOrDefault(p => p.UserName == username);
        }
        public Room GetRoomByName(string roomname)
        {
            return context.Rooms.FirstOrDefault(p => p.RoomName == roomname);
        }

        public User GetUserByID(int id)
        {
            return context.Users.FirstOrDefault(p => p.UserID == id);
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
            var room = GetRoomByName(roomname);
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
            return context.Rooms.FirstOrDefault(p => p.RoomID == id);
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

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
