using ChatWithSignalR.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Repo
{
    public class DataManagerAsync : IDisposable
    {
        ChatDataEntities context;
        public DataManagerAsync()
        {
            context = new ChatDataEntities();
        }

        public async Task<int> InsertUser(string UserName, string eMail, string Password, string token, bool active)
        {
            return await Task.Run(() =>
            {
                return context.sp_InsertIntoUsers(UserName, eMail, Password.GetHashCode().ToString(), token, active);
            });

        }
        public async Task<int> InsertMessage(Guid guid, string username, string roomname, string message)
        {
            return await Task.Run(() => context.sp_InsertIntoMessages(guid, username, roomname, message, DateTime.Now));
        }
        public async Task<int> EditMessage(Guid id, string newmessage)
        {
            return await Task.Run(() => context.sp_EditMessage(id, newmessage, DateTime.Now));
        }

        public async Task<int> InsertRoom(string roomname)
        {
            return await Task.Run(() => context.sp_InsertIntoRooms(roomname));
        }
        public async Task<bool> IsValidNameOrEmail(string username, string email)
        {
            return await context.Database.SqlQuery<bool>("select dbo.f_IsValidNameOrEmail('" + username + "','" + email + "')").FirstOrDefaultAsync();
        }
        public async Task<int> OutFromRoom(string username, string roomname)
        {
            return await Task.Run(() => context.sp_OutFromRoom(username, roomname));
        }
        public async Task<int> JoinGroup(string username, string roomname)
        {
            return await Task.Run(() => context.sp_JoinGroup(username, roomname));
        }
        public async Task<bool> IsValidRoomName(string roomname)
        {
            return await context.Database.SqlQuery<bool>("select dbo.f_IsValidRoomName('" + roomname + "')").FirstOrDefaultAsync();
        }
        public async Task<bool> Login(string username, string password)
        {
            return await context.Database.SqlQuery<bool>("select dbo.f_Login('" + username + "','" + password + "')").FirstOrDefaultAsync();
        }
        public async Task<bool> RoomContainsUser(string username, string roomname)
        {
            return await context.Database.SqlQuery<bool>("select dbo.f_RoomContainsUser('" + username + "','" + roomname + "')").FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByName(string username)
        {
            return await context.Users.FirstOrDefaultAsync(p => p.UserName == username);
        }
        public async Task<Room> GetRoomByName(string roomname)
        {
            return await context.Rooms.FirstOrDefaultAsync(p => p.RoomName == roomname);
        }
        public async Task<User> GetUserByID(int id)
        {
            return await context.Users.FirstOrDefaultAsync(p => p.UserID == id);
        }
        public async Task<List<OldMessage>> GetHistory(string id)
        {
            return await Task.Run(() => context.OldMessages.Select(p => p).Where(p => p.ID.ToString() == id).ToList());
        }
        public async Task<List<User>> GetUsersByRoom(string roomname)
        {
            return await Task.Run(async () =>
            {
                List<User> users = new List<User>();
                var room = await GetRoomByName(roomname);
                var id = room.RoomID;
                var items = context.UsersRooms.Select(p => p).Where(p => p.RoomID == id).ToList();
                foreach (var item in items)
                {
                    users.Add(GetUserByID(item.UserID).Result);
                }
                return users;
            });
        }

        public async Task<List<Message>> GetMessagesByRoomName(string roomname)
        {
            var room = await GetRoomByName(roomname);
            int id = room.RoomID;
            return context.Messages.Select(p => p).Where(p => p.RoomID == id).ToList();
        }
        public async Task<Message> GetMessageByID(Guid id)
        {
            return await context.Messages.FirstOrDefaultAsync(p => p.ID == id);
        }
        public async Task<List<Room>> GetRooms()
        {
            return await Task.Run(() => context.Rooms.ToList());
        }
        public async Task<Room> GetRoomByID(int id)
        {
            return await context.Rooms.FirstOrDefaultAsync(p => p.RoomID == id);
        }

        public async Task<List<int>> GetRoomIDsByUser(string username)
        {
            return await Task.Run(async () =>
            {
                List<int> ids = new List<int>();
                int id = await context.Users.Where(p => p.UserName == username).Select(p => p.UserID).FirstOrDefaultAsync();
                var items = context.UsersRooms.Select(p => p).Where(p => p.UserID == id).ToList();
                foreach (var item in items)
                {
                    ids.Add(item.RoomID);
                }
                return ids;
            });
        }
        public async Task<bool> CheckingActivation(string token)
        {
            if (context.Users.Where(p => p.token == token).Count() == 1 && context.Users.Where(p => p.token == token).First().active == false)
            {
                context.Users.Where(p => p.token == token).First().active = true;
                context.Users.Where(p => p.token == token).First().token = Guid.NewGuid().ToString();
                await context.SaveChangesAsync();
                return true;

            }
            else
                return false;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}


