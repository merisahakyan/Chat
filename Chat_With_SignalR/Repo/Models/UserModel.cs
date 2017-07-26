using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Models
{
    public class UserModel
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string eMail { get; set; }
        public string Password { get; set; }
        public string token { get; set; }
        public bool active { get; set; }
    }
}
