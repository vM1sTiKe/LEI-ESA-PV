using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    [Authorize]
    public class RestaurantesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}