using AeroBites.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    [Authorize]
    public class MyRestaurantController : Controller
    {
        private readonly AeroBitesContext _context;

        public MyRestaurantController(AeroBitesContext context) {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
