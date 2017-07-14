using System.Web.Mvc;

namespace Simple_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Room(string id,string userid,string username,string roomname)
        {
            ViewData["roomid"] = id;
            
            ViewData["userid"] = userid;
            ViewData["roomname"] = roomname;

            ViewData["username"] = username;
            return View();
        }
    }
}