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
        public void InsertMessage(Guid guid, string username, string roomname, string message)
        {
            context.InsertIntoMessages(guid, username, roomname, message,DateTime.Now);
        }
        public void EditMessage(Guid id, string newmessage)
        {
            context.EditMessage(id, newmessage,DateTime.Now);
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
        public UserModel GetUserByName(string username)
        {
            return context.Database.SqlQuery<UserModel>("select * from Users where UserName='" + username + "'").FirstOrDefault();
        }
        public RoomModel GetRoomByName(string roomname)
        {
            return context.Database.SqlQuery<RoomModel>("select * from Rooms where RoomName='" + roomname + "'").FirstOrDefault();
        }

        public UserModel GetUserByID(int id)
        {
            return context.Database.SqlQuery<UserModel>("select * from Users where UserID=" + id).FirstOrDefault();
        }
        public List<HistoryModel> GetHistory(string id)
        {
            List<HistoryModel> history = new List<HistoryModel>();
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from OldMessages where ID='" + id.ToString().ToUpper() + "'", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            HistoryModel model = new HistoryModel();
                            model.Message = r["MessageText"].ToString();
                            model.Edited = (DateTime)r["Time"];
                            history.Add(model);
                        }
                    }
                }
            }
            return history;
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
                            msg.ID = (Guid)r["ID"];
                            msg.Time = (DateTime)r["DateTime"];
                            messages.Add(msg);
                        }
                    }
                }
            }
            return messages;
        }
        public MessageModel GetMessageByID(Guid id)
        {
            MessageModel msg = new MessageModel();
            using (SqlConnection conn = new SqlConnection("data source=F4FS-01132;initial catalog=ChatData;integrated security=True"))
            {
                using (SqlCommand cmd = new SqlCommand("select * from [Messages] where ID='" + id.ToString().ToUpper() + "'", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        msg.RoomName = GetRoomByID((int)r["RoomID"]).RoomName;
                        msg.UserName = GetUserByID((int)r["UserID"]).UserName;
                        msg.Message = r["MessageText"].ToString();
                        msg.ID = (Guid)r["ID"];
                        msg.Time = (DateTime)r["DateTime"];
                    }
                }
            }
            return msg;
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
            return context.Database.SqlQuery<RoomModel>("select * from Rooms where RoomID=" + id).FirstOrDefault();
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
