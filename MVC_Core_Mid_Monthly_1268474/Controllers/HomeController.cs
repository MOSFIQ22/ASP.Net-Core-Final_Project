using Microsoft.AspNetCore.Mvc;

namespace Practice_mvc_core_01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
