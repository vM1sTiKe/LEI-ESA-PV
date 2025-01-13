using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class RestaurantesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}