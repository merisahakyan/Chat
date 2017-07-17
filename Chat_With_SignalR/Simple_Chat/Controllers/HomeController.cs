using System.Web.Mvc;

namespace Simple_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Room(string roomname)
        {
            ViewData["roomname"] = roomname;
            
            return View();
        }
    }
}