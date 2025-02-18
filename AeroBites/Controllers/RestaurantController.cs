using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AeroBites.Data;
using Microsoft.EntityFrameworkCore;

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
            var restaurants = _context.Restaurant.Where(restaurant => restaurant.Status == Enums.RestaurantStatus.Valid).ToList();
            return View(restaurants);
        }       

        public IActionResult Menu(int id)
        {
            var restaurant = _context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.Id == id);
            return View(restaurant);
        }
    }
}