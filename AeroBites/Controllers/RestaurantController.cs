using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AeroBites.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AeroBites.Controllers
{
    [Authorize]
    public class RestaurantController(AeroBitesContext context) : Controller
    {
        /// <summary>
        /// Displays a list of valid restaurants that are currently available.
        /// Filters out restaurants that are not validated yet.
        /// </summary>
        /// <returns>
        /// A view with a list of valid restaurants.
        /// </returns>
        public IActionResult Index() {
            var userId = User.GetId();

            var addresses = context.Address.Where(a => a.AccountId == userId).ToList();

            var activeAddressId = addresses.FirstOrDefault(a => a.IsActive)?.Id;

            ViewBag.Addresses = new SelectList(addresses, "Id", "FullAddress", activeAddressId);
            ViewBag.ActiveAddressId = activeAddressId;

            var restaurants = context.Restaurant.Where(restaurant => restaurant.Status == Enums.RestaurantStatus.Valid).ToList();
            return View(restaurants);
        }

        /// <summary>
        /// Displays the menu of a specific restaurant, including its categories and items.
        /// </summary>
        /// <param name="id">The unique identifier of the restaurant.</param>
        /// <returns>
        /// A view displaying the restaurant's menu, including categories and items.
        /// </returns>
        public IActionResult Menu(int id)
        {
            var restaurant = context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.Id == id);
            ViewBag.Cart = context.Cart.Include(c => c.Items).Where(c => c.AccountId == User.GetId() && c.Status == Enums.OrderStatus.Choosing).FirstOrDefault();
            return View(restaurant);
        }


        [HttpPost]
        public IActionResult SetActiveAddress(int addressId)
        {
            var userId = User.GetId();

            var addresses = context.Address.Where(a => a.AccountId == userId).ToList();
            foreach (var address in addresses)
            {
                address.IsActive = address.Id == addressId;
            }

            context.SaveChanges();

            return Ok();
        }
    }
}