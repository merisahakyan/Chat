using AutoMapper;
using BLL.Models;
using Repo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL
{
    public class Manager : IManager
    {
         IDataManager _datamanager;
         IMapper imapper;

        IDataManager datamanager {
            get
            {
                if (_datamanager == null)
                    _datamanager = new DataManager();
                return _datamanager;
            }
            set
            {
                _datamanager=value;
            }
        }

        public Manager() 
        {
            //datamanager = new DataManager();
            imapper = AutoMapperConfiguration.GetMapper();
        }
        
        public async Task InsertUser(string UserName, string eMail, string Password, string token, bool active)
        {
            await datamanager.InsertUser(UserName, eMail, Password, token, active);
        }
        public async Task InsertMessage(Guid guid, string username, string roomname, string message)
        {
            await datamanager.InsertMessage(guid, username, roomname, message);
        }
        public async Task EditMessage(Guid id, string newmessage)
        {
            await datamanager.EditMessage(id, newmessage);
        }
        public async Task InsertRoom(string roomname)
        {
            await datamanager.InsertRoom(roomname);
        }
        public async Task<bool> IsValidNameOrEmail(string username, string email)
        {
            return await datamanager.IsValidNameOrEmail(username, email);
        }
        public async Task OutFromRoom(string username, string roomname)
        {
            await datamanager.OutFromRoom(username, roomname);
        }
        public async Task JoinGroup(string username, string roomname)
        {
            await datamanager.JoinGroup(username, roomname);
        }
        public async Task<bool> IsValidRoomName(string roomname)
        {
            return await datamanager.IsValidRoomName(roomname);
        }
        public async Task<bool> Login(string username, string password)
        {
            return await datamanager.Login(username, password.GetHashCode().ToString());
        }
        public async Task<bool> RoomContainsUser(string username, string roomname)
        {
            return await datamanager.RoomContainsUser(username, roomname);
        }
        public async Task<UserModel> GetUserByName(string username)
        {
            return imapper.Map<UserModel>(await datamanager.GetUserByName(username));
        }
        public async Task<RoomModel> GetRoomByName(string roomname)
        {
            return imapper.Map<RoomModel>(await datamanager.GetRoomByName(roomname));
        }

        public async Task<List<HistoryModel>> GetHistory(string id)
        {
            return imapper.Map<List<HistoryModel>>(await datamanager.GetHistory(id));
        }
        public async Task<List<UserModel>> GetUsersByRoom(string roomname)
        {
            return imapper.Map<List<UserModel>>(await datamanager.GetUsersByRoom(roomname));
        }

        public async Task<List<MessageModel>> GetMessagesByRoomName(string roomname)
        {
            return imapper.Map<List<MessageModel>>(await datamanager.GetMessagesByRoomName(roomname));
        }
        public async Task<MessageModel> GetMessageByID(Guid id)
        {
            return imapper.Map<MessageModel>(await datamanager.GetMessageByID(id));
        }
        public async Task<List<RoomModel>> GetRooms(string username)
        {
            return await Task.Run(async () =>
            {
                var allrooms = await datamanager.GetRooms();
                List<RoomModel> joinedrooms = await GetRoomsByUser(username);
                List<RoomModel> rooms = new List<RoomModel>();
                foreach (var item in allrooms)
                {
                    RoomModel room = imapper.Map<RoomModel>(item);
                    if (!joinedrooms.Contains(room))
                        rooms.Add(room);
                }
                return rooms;
            });
        }

        public async Task<List<RoomModel>> GetRoomsByUser(string username)
        {
            return await Task.Run(async () =>
            {
                List<int> ids = await datamanager.GetRoomIDsByUser(username);
                List<RoomModel> rooms = new List<RoomModel>();
                foreach (var item in ids)
                {
                    rooms.Add(imapper.Map<RoomModel>(await datamanager.GetRoomByID(item)));
                }
                return rooms;
            });
        }
        public async Task<string> CheckingActivation(string token)
        {
            var t = await datamanager.CheckingActivation(token);
            if (t)
                return "You'r activated your account!";
            else
                return
                    "Something went wrong!";
        }
        public void LogOut()
        {
            datamanager.Dispose();
        }
    }
}
