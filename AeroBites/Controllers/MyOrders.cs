using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class MyOrders : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }
    }
}
