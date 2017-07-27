using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class UserModel
    {
        public UserModel()
        {

        }
        public UserModel(User user)
        {
            UserName = user.UserName;
            eMail = user.eMail;
            Password = user.Password;
            token = user.token;
            active = user.active;
        }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string eMail { get; set; }
        public string Password { get; set; }
        public string token { get; set; }
        public bool active { get; set; }
    }
}
