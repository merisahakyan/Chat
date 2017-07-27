using AutoMapper;
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
        IMapper imapper;
        public Manager()
        {
            imapper = AutoMapperConfiguration.GetMapper();
        }
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
            return datamanager.Login(username, password.GetHashCode().ToString());
        }
        public bool RoomContainsUser(string username, string roomname)
        {
            return datamanager.RoomContainsUser(username, roomname);
        }
        public UserModel GetUserByName(string username)
        {
           return imapper.Map<UserModel>(datamanager.GetUserByName(username));
        }
        public RoomModel GetRoomByName(string roomname)
        {
            return imapper.Map<RoomModel>(datamanager.GetRoomByName(roomname));
        }

        public UserModel GetUserByID(int id)
        {
            return imapper.Map<UserModel>(datamanager.GetUserByID(id));
        }
        public List<HistoryModel> GetHistory(string id)
        {
            return imapper.Map<List<HistoryModel>>(datamanager.GetHistory(id));
        }
        public List<UserModel> GetUsersByRoom(string roomname)
        {
            return imapper.Map<List<UserModel>>(datamanager.GetUsersByRoom(roomname));
        }

        public List<MessageModel> GetMessagesByRoomName(string roomname)
        {
            return imapper.Map<List<MessageModel>>(datamanager.GetMessagesByRoomName(roomname));
        }
        public MessageModel GetMessageByID(Guid id)
        {
            return imapper.Map<MessageModel>(datamanager.GetMessageByID(id));
        }
        public List<RoomModel> GetRooms(string username)
        {
            var items = datamanager.GetRooms();
            List<RoomModel> jr = GetRoomsByUser(username);
            List<RoomModel> rooms = new List<RoomModel>();
            foreach (var item in items)
            {
                RoomModel room = imapper.Map<RoomModel>(item);
                if (!jr.Contains(room))
                    rooms.Add(room);
            }
            return rooms;
        }
        public RoomModel GetRoomByID(int id)
        {
            return imapper.Map<RoomModel>(datamanager.GetRoomByID(id));
        }
        public List<RoomModel> GetRoomsByUser(string username)
        {
            return imapper.Map<List<RoomModel>>(datamanager.GetRoomsByUser(username));
        }
        public string CheckingActivation(string token)
        {
            return datamanager.CheckingActivation(token);
        }
        public void LogOut()
        {
            datamanager.Dispose();
        }
    }
}
