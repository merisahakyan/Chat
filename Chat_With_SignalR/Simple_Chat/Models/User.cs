namespace Simple_Chat.Models
{
    public class User
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
    }
}