using Microsoft.AspNetCore.Mvc;

namespace Compras.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
