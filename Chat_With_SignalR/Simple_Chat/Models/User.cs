using System;

namespace Simple_Chat.Models
{
    public class User : IEquatable<User>
    {
        public User()
        {

        }
        public User(Simple_Chat.User user)
        {
            this.Name = user.UserName;
            this.Password = user.Password;
            this.eMail = user.eMail;
            this.token = user.token;
            this.active = user.active;
        }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string eMail { get; set; }
        public string token { get; set; }
        public bool active { get; set; }

        public bool Equals(User other)
        {
            if (other == null) return false;
            return (this.Name == other.Name);
        }
    }
}