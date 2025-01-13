using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
