using ChatWithSignalR.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public interface IDataManager:IDisposable
    {
        Task<int> InsertUser(string UserName, string eMail, string Password, string token, bool active);
        Task<int> InsertMessage(Guid guid, string username, string roomname, string message);
        Task<int> EditMessage(Guid id, string newmessage);
        Task<int> InsertRoom(string roomname);
        Task<bool> IsValidNameOrEmail(string username, string email);
        Task<int> OutFromRoom(string username, string roomname);
        Task<int> JoinGroup(string username, string roomname);
        Task<bool> IsValidRoomName(string roomname);
        Task<bool> Login(string username, string password);
        Task<bool> RoomContainsUser(string username, string roomname);
        Task<User> GetUserByName(string username);
        Task<Room> GetRoomByName(string roomname);
        Task<User> GetUserByID(int id);
        Task<List<OldMessage>> GetHistory(string id);
        Task<List<User>> GetUsersByRoom(string roomname);
        Task<List<Message>> GetMessagesByRoomName(string roomname);
        Task<Message> GetMessageByID(Guid id);
        Task<List<Room>> GetRooms();
        Task<Room> GetRoomByID(int id);
        Task<List<int>> GetRoomIDsByUser(string username);
        Task<bool> CheckingActivation(string token);
       
    }
}
