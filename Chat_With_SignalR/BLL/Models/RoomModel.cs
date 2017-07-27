using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class RoomModel : IEquatable<RoomModel>
    {
        public RoomModel()
        {
            Users = new List<UserModel>();
        }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public List<UserModel> Users { get; set; }

        public bool Equals(RoomModel other)
        {
            if (this.RoomID == other.RoomID)
                return true;
            else
                return false;
        }
    }
}
