using BLL.Models;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IManager
    {
        Task InsertUser(string UserName, string eMail, string Password, string token, bool active);
        Task InsertMessage(Guid guid, string username, string roomname, string message);
        Task EditMessage(Guid id, string newmessage);
        Task InsertRoom(string roomname);
        Task<bool> IsValidNameOrEmail(string username, string email);
        Task OutFromRoom(string username, string roomname);
        Task JoinGroup(string username, string roomname);
        Task<bool> IsValidRoomName(string roomname);
        Task<bool> Login(string username, string password);
        Task<bool> RoomContainsUser(string username, string roomname);
        Task<UserModel> GetUserByName(string username);
        Task<RoomModel> GetRoomByName(string roomname);
        Task<List<HistoryModel>> GetHistory(string id);
        Task<List<UserModel>> GetUsersByRoom(string roomname);
        Task<List<MessageModel>> GetMessagesByRoomName(string roomname);
        Task<MessageModel> GetMessageByID(Guid id);
        Task<List<RoomModel>> GetRooms(string username);
        Task<List<RoomModel>> GetRoomsByUser(string username);
        Task<string> CheckingActivation(string token);
        void LogOut();
    }
}
