using System;
using System.Linq;
using System.Web.Mvc;

namespace Simple_Chat.Controllers
{
    public class HomeController : Controller
    {
        ChatDataContext _context = new ChatDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Room(string roomname, string username)
        {
            string room = roomname;
            if (room.Length > 17)
                room = roomname.Substring(0, 17) + "...";
            ViewData["roomname"] = room;
            ViewData["roomtitle"] = roomname;
            ViewData["username"] = username;
            return View();
        }
        public ActionResult ActivatePage(string token)
        {
            if (_context.Users.Where(p => p.token == token).Count() == 1 && _context.Users.Where(p => p.token == token).First().active == false)
            {
                _context.Users.Where(p => p.token == token).First().active = true;
                _context.Users.Where(p => p.token == token).First().token = Guid.NewGuid().ToString();
                _context.SaveChanges();
                ViewData["activationmessage"] = "You'r activated your account!";

            }
            else
                ViewData["activationmessage"] = "Something went wrong!";
            return View();

        }
    }
}