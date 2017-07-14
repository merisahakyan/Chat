using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Chat.Models
{
    public class Room
    {
        public Guid RoomID { get; set; }
        public string RoomName { get; set; }
        public List<User> Members { get; set; }
    }
}
