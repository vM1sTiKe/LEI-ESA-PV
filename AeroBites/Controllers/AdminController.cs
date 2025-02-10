using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly AeroBitesContext _context;

        public AdminController(AeroBitesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ApproveRestaurant(int id)
        {
            var restaurant = _context.Restaurant.Find(id);

            restaurant.Status = Enums.RestaurantStatus.Valid;
            _context.SaveChanges();

            TempData["RequestMessage"] = "Restaurante aprovado!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DenyeRestaurant(int id)
        {
            var restaurant = _context.Restaurant.Find(id);

            _context.Restaurant.Remove(restaurant);
            _context.SaveChanges();

            TempData["RequestMessage"] = "Restaurante negado!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public List<Restaurant> GetAllRestaurants()
        {
            var validRestaurantes = _context.Restaurant.ToList();
            return validRestaurantes;
        }
    }
}
