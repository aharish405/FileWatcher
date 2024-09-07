using Microsoft.AspNetCore.Mvc;

namespace FileWatcherApp.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
