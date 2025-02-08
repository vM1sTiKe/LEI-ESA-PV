using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroBites.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly AeroBitesContext _context;

        public UserController(AeroBitesContext context)
        {
            _context = context;
        }

        public IActionResult MyRestaurant()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRestaurant(string name)
        {
            Console.WriteLine("Restaurant Name: " + name);
            if (RestaurantNameExists(name))
            {
                TempData["RequestMessage"] = "Nome do resturante em uso!";
                return RedirectToAction(nameof(MyRestaurant));
            }

            var newRestaurant = new Restaurant
            {
                Name = name,
                Status = Enums.RestaurantStatus.WaitingAcceptance,
                OwnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            };

            _context.Restaurant.Add(newRestaurant);
            _context.SaveChanges();

            TempData["RequestMessage"] = "Pedido de criação submetdio e à espera de aprovação.";

            return RedirectToAction(nameof(MyRestaurant));
        }

        private bool RestaurantNameExists(string name)
        {
            return _context.Restaurant.Any(restaurant => restaurant.Name == name);
        }
    }
}
