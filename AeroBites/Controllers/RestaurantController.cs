using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AeroBites.Data;

namespace AeroBites.Controllers
{
    [Authorize]
    public class RestaurantController : Controller
    {
        private readonly AeroBitesContext _context;

        public RestaurantController(AeroBitesContext context)
        {
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }       

        public IActionResult Menu()
        {
            return View();
        }

        [HttpGet]
        public List<Restaurant> GetValidRestaurants()
        {
            var validRestaurantes = _context.Restaurant.Where(restaurant => restaurant.Status == Enums.RestaurantStatus.Valid).ToList();
            return validRestaurantes;
        }

    }
}