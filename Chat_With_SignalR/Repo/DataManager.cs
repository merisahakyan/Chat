using Repo.Models;
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
        public void InsertMessage(string username, string roomname, string message)
        {
            context.InsertIntoMessages(username, roomname, message);
        }
        public void InsertRoom(string roomname)
        {
            context.InsertIntoRooms(roomname);
        }
        public bool IsValidNameOrEmail(string username, string email)
        {
            
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select dbo.IsValidNameOrEmail('" + username + "','" + email + "')", conn))
                {
                    conn.Open();
                    return (bool)cmd.ExecuteScalar();
                }
            }

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
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select dbo.IsValidRoomName('" + roomname + "')", conn))
                {
                    conn.Open();
                    return (bool)cmd.ExecuteScalar();
                }
            }
        }
        public bool Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select dbo.Login('" + username + "','" + password + "')", conn))
                {
                    conn.Open();
                    return (bool)cmd.ExecuteScalar();
                }
            }
        }
        public bool RoomContainsUser(string username, string roomname)
        {
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select dbo.RoomContainsUser('" + username + "','" + roomname + "')", conn))
                {
                    conn.Open();
                    return (bool)cmd.ExecuteScalar();
                }
            }
        }
        public UserModel GetUserByName(string username)
        {
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(@"select * from Users where UserName='" + username + "'", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        UserModel user = new UserModel();
                        user.UserName = r["UserName"].ToString();
                        user.eMail = r["eMail"].ToString();
                        user.Password = r["Password"].ToString();
                        user.token = r["token"].ToString();
                        user.active = (bool)r["active"];
                        return user;
                    }
                }
            }
        }
        public RoomModel GetRoomByName(string roomname)
        {
            RoomModel room = new RoomModel();
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(@"select * from Rooms where RoomName='" + roomname + "'", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        room.RoomName = r["RoomName"].ToString();
                        room.RoomID = (int)r["RoomID"];
                        return room;
                    }
                }
            }
        }

        public UserModel GetUserByID(int id)
        {
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(@"select * from Users where UserID=" + id, conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        UserModel user = new UserModel();
                        user.UserName = r["UserName"].ToString();
                        user.eMail = r["eMail"].ToString();
                        user.Password = r["Password"].ToString();
                        user.token = r["token"].ToString();
                        user.active = (bool)r["active"];
                        return user;
                    }
                }
            }
        }

        public List<UserModel> GetUsersByRoom(string roomname)
        {
            List<UserModel> users = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from UsersRooms where RoomID=" + GetRoomByName(roomname).RoomID + "", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            users.Add(GetUserByID((int)r["UserID"]));
                        }
                    }
                }
            }
            return users;
        }

        public List<MessageModel> GetMessagesByRoomName(string roomname)
        {
            List<MessageModel> messages = new List<Models.MessageModel>();
            int id = GetRoomByName(roomname).RoomID;
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from Messages where RoomID=" + id, conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            MessageModel msg = new MessageModel();
                            msg.RoomName = roomname;
                            msg.UserName = GetUserByID((int)r["UserID"]).UserName;
                            msg.Message = r["MessageText"].ToString();
                            messages.Add(msg);
                        }
                    }
                }
            }
            return messages;
        }
        public List<RoomModel> GetRooms(string username)
        {

            List<RoomModel> rooms = new List<RoomModel>();
            List<RoomModel> jr = GetRoomsByUser(username);
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(@"select * from Rooms", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            RoomModel room = new RoomModel();
                            room.RoomName = r["RoomName"].ToString();
                            room.RoomID = (int)r["RoomID"];
                            room.Users = GetUsersByRoom(r["RoomName"].ToString());
                            if (!jr.Contains(room))
                                rooms.Add(room);
                        }
                    }
                }
            }
            return rooms;
        }
        public RoomModel GetRoomByID(int id)
        {
            RoomModel room = new RoomModel();
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(@"select * from Rooms where RoomID='" + id + "'", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        room.RoomID = (int)r["RoomID"];
                        room.RoomName = r["RoomName"].ToString();
                        room.Users = GetUsersByRoom(r["RoomName"].ToString());
                    }
                }
            }
            return room;
        }
        public List<RoomModel> GetRoomsByUser(string username)
        {
            int id;
            List<RoomModel> rooms = new List<Models.RoomModel>();
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(@"select * from Users where UserName='" + username + "'", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        id = (int)r["UserID"];
                    }
                }
            }
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(@"select * from UsersRooms where UserID=" + id, conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            rooms.Add(GetRoomByID((int)r["RoomID"]));
                        }
                    }
                }
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
