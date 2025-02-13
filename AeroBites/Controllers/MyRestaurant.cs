using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class MyRestaurant : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
