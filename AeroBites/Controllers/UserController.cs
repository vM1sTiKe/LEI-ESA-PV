using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            Restaurant restaurant = GetRestaurant();
            return View(restaurant);
        }

        [HttpPost]
        public IActionResult CreateRestaurant(string name)
        {
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

            TempData["RequestMessage"] = "Pedido de criação submetido.";

            return RedirectToAction(nameof(MyRestaurant));
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

        [HttpPost]
        public IActionResult EditItem(int id, string name, int price)
        {
            var item = _context.Item.FirstOrDefault(item => item.Id == id);
            
            if (ItemExists(name) && item.Name != name)
            {
                TempData["RequestMessage"] = "Item já existente";
                return RedirectToAction(nameof(MyRestaurant));
            }

            item.Name = char.ToUpper(name[0]) + name.Substring(1);
            item.Price = price;
            _context.SaveChanges();

            TempData["RequestMessage"] = "Item editado com sucesso.";

            return RedirectToAction(nameof(MyRestaurant));
        }

        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            var item = _context.Item.FirstOrDefault(item => item.Id == id);

            if (item!=null)
            {
                TempData["RequestMessage"] = "Item não existe";
                return RedirectToAction(nameof(MyRestaurant));
            }

            _context.Remove(item);
            _context.SaveChanges();

            TempData["RequestMessage"] = "Item eliminado com sucesso.";

            return RedirectToAction(nameof(MyRestaurant));
        }

        private Restaurant? GetRestaurant()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _context.Restaurant.Include(r => r.Items).FirstOrDefault(r => r.OwnerId == userId);
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
