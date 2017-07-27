using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class MessageModel
    {
        public Guid ID { get; set; }
        public string UserName { get; set; }
        public string RoomName { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
