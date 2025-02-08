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
            if (RestaurantNameExists(name))
            {
                return Conflict(new { message = "Nome do resturante já registado!" });
            }

            var newRestaurant = new Restaurant
            {
                Name = name,
                Status = Enums.RestaurantStatus.WaitingAcceptance,
                OwnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            };

            _context.Restaurant.Add(newRestaurant);
            _context.SaveChanges();

            return Ok(new { message = "Pedido de criação submetdio e à espera de aprovação." });
        }

        [HttpPost]
        public IActionResult AddItem(string name, int price)
        {
            if (ItemExists(name))
            {
                TempData["RequestMessage"] = "Item já existente";
                return RedirectToAction(nameof(MyRestaurant));
            }

            Item item = new Item
            {
                Name = char.ToUpper(name[0]) + name.Substring(1),
                Price = price,
                RestaurantId = GetRestaurant().Id
            };

            _context.Add(item);
            _context.SaveChanges();

            TempData["RequestMessage"] = "Item criado com sucesso.";

            return RedirectToAction(nameof(MyRestaurant));
        }

        private Restaurant? GetRestaurant()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _context.Restaurant.FirstOrDefault(restaurant => restaurant.OwnerId == userId);
        }

        private bool ItemExists(string name)
        {
            return _context.Item.Any(item => item.Name == name);
        }

        private bool RestaurantNameExists(string name)
        {
            return _context.Restaurant.Any(restaurant => restaurant.Name == name);
        }
    }
}
