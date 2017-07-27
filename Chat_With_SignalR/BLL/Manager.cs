using BLL.Models;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Manager
    {
        DataManager datamanager = new DataManager();
        public void InsertUser(string UserName, string eMail, string Password, string token, bool active)
        {
            datamanager.InsertUser(UserName, eMail, Password, token, active);
        }
        public void InsertMessage(Guid guid, string username, string roomname, string message)
        {
            datamanager.InsertMessage(guid, username, roomname, message);
        }
        public void EditMessage(Guid id, string newmessage)
        {
            datamanager.EditMessage(id, newmessage);
        }

        public void InsertRoom(string roomname)
        {
            datamanager.InsertRoom(roomname);
        }
        public bool IsValidNameOrEmail(string username, string email)
        {
            return datamanager.IsValidNameOrEmail(username, email);
        }
        public void OutFromRoom(string username, string roomname)
        {
            datamanager.OutFromRoom(username, roomname);
        }
        public void JoinGroup(string username, string roomname)
        {
            datamanager.JoinGroup(username, roomname);
        }
        public bool IsValidRoomName(string roomname)
        {
            return datamanager.IsValidRoomName(roomname);
        }
        public bool Login(string username, string password)
        {
            return datamanager.Login(username, password);
        }
        public bool RoomContainsUser(string username, string roomname)
        {
            return datamanager.RoomContainsUser(username, roomname);
        }
        public UserModel GetUserByName(string username)
        {
            return new UserModel(datamanager.GetUserByName(username));

        }
        public RoomModel GetRoomByName(string roomname)
        {
            return new RoomModel(datamanager.GetRoomByName(roomname));
        }

        public UserModel GetUserByID(int id)
        {
            return new UserModel(datamanager.GetUserByID(id));
        }
        public List<HistoryModel> GetHistory(string id)
        {
            List<HistoryModel> history = new List<HistoryModel>();
            var items = datamanager.GetHistory(id);
            foreach (var item in items)
            {
                history.Add(new HistoryModel() { Message = item.MessageText, Edited = item.Time });
            }
            return history;
        }
        public List<UserModel> GetUsersByRoom(string roomname)
        {
            var users = datamanager.GetUsersByRoom(roomname);
            List<UserModel> rusers = new List<UserModel>();
            foreach (var item in users)
            {
                rusers.Add(new UserModel(item));
            }
            return rusers;
        }

        public List<MessageModel> GetMessagesByRoomName(string roomname)
        {
            List<MessageModel> messages = new List<Models.MessageModel>();
            var items = datamanager.GetMessagesByRoomName(roomname);
            foreach (var item in items)
            {
                messages.Add(new MessageModel()
                {
                    ID = item.ID,
                    Message = item.MessageText,
                    RoomName = GetRoomByID(item.RoomID).RoomName,
                    Time = item.DateTime,
                    UserName = GetUserByID(item.UserID).UserName
                });
            }
            return messages;
        }
        public MessageModel GetMessageByID(Guid id)
        {
            var msg = datamanager.GetMessageByID(id);
            return new MessageModel()
            {
                ID = msg.ID,
                Message = msg.MessageText,
                RoomName = msg.Room.RoomName,
                Time = msg.Time,
                UserName = msg.User.UserName
            };
        }
        public List<RoomModel> GetRooms(string username)
        {
            var items = datamanager.GetRooms();
            
            List<RoomModel> jr = GetRoomsByUser(username);
            List<RoomModel> rooms = new List<RoomModel>();
            
            foreach (var item in items)
            {
                RoomModel room = new RoomModel()
                {
                    RoomID = item.RoomID,
                    RoomName = item.RoomName,
                    Users = GetUsersByRoom(item.RoomName)
                };
                if (!jr.Contains(room))
                    rooms.Add(room);
            }
            return rooms;
        }
        public RoomModel GetRoomByID(int id)
        {
            return new RoomModel(datamanager.GetRoomByID(id));
        }
        public List<RoomModel> GetRoomsByUser(string username)
        {

            List<RoomModel> rooms = new List<RoomModel>();
            var items = datamanager.GetRoomsByUser(username);
            foreach (var item in items)
            {
                rooms.Add(new RoomModel(item));
            }
            return rooms;
        }
        public string CheckingActivation(string token)
        {
            return datamanager.CheckingActivation(token);
        }
    }
}
