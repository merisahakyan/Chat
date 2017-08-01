
using BLL;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Simple_Chat.Controllers
{
    public class HomeController : Controller
    {
        ManagerAsync manager = new ManagerAsync();
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
            ViewData["activationmessage"] = manager.CheckingActivation(token);
            return View();

        }
    }
}