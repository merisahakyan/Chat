using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Chat.Models
{
    public class Message
    {
        public Message(Simple_Chat.Message m)
        {
            this.Sender = m.User.UserName;
            this.Text = m.MessageText;
        }
        public string Sender { get; set; }
        public string Text { get; set; }
    }
}
