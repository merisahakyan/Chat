using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Chat.Models
{
    public class Room : IEquatable<Room>
    {
        public Room()
        {

        }
        public Room(Simple_Chat.Room room)
        {
            this.RoomID = room.RoomID;
            this.RoomName = room.RoomName;
        }
        public int RoomID { get; set; }
        public string RoomName { get; set; }

        public bool Equals(Room other)
        {
            if (other == null) return false;
            return (this.RoomID == other.RoomID);
        }
    }
}
