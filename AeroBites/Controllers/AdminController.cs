using AeroBites.Data;
using AeroBites.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController(AeroBitesContext context) : Controller
    {
        /// <summary>
        /// Displays a list of restaurants pending approval, as well as already approved ones for management actions.
        /// </summary>
        /// <returns>
        /// A view displaying the list of restaurants for approval or deletion.
        /// </returns>
        public IActionResult Restaurants()
        {
            return View(context.Restaurant.ToList());
        }

        /// <summary>
        /// Displays a list of all available delivery points.
        /// </summary>
        /// <returns>
        /// A view showing all delivery points.
        /// </returns>

        /// <summary>
        /// Approves a restaurant by setting its status to Valid.
        /// </summary>
        /// <param name="id">The ID of the restaurant to approve.</param>
        /// <returns>
        /// A redirect to the Index page with a success message.
        /// </returns>
        [HttpGet]
        public IActionResult ApproveRestaurant(int id)
        {
            var restaurant = context.Restaurant.Find(id);

            restaurant.Status = Enums.RestaurantStatus.Valid;
            context.SaveChanges();

            TempData[Enums.MessageType.successMessage.ToString()] = "Restaurante aprovado.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Denies a restaurant request by removing it from the database.
        /// </summary>
        /// <param name="id">The ID of the restaurant to deny.</param>
        /// <returns>
        /// A redirect to the Index page with a success message.
        /// </returns>
        [HttpGet]
        public IActionResult DenyRestaurant(int id)
        {
            var restaurant = context.Restaurant.Find(id);

            context.Restaurant.Remove(restaurant);
            context.SaveChanges();

            TempData[Enums.MessageType.successMessage.ToString()] = "Restaurante negado.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Deletes a restaurant permanently from the database.
        /// </summary>
        /// <param name="id">The ID of the restaurant to delete.</param>
        /// <returns>
        /// A redirect to the Index page with a success message.
        /// </returns>
        [HttpGet]
        public IActionResult DeleteRestaurant(int id)
        {
            var restaurant = context.Restaurant.FirstOrDefault(restaurant => restaurant.Id == id);

            context.Restaurant.Remove(restaurant);
            context.SaveChanges();

            TempData[Enums.MessageType.successMessage.ToString()] = "Restaurante eliminado.";

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Returns the view to visualize statistics and give needed data to populate the view
        /// </summary>
        public IActionResult Statistics()
        {
            // Daily Sells
            List<DailySells> dailySells = [];
            // Get all sells and iterate them
            foreach (var cart in context.Cart.Where(c => c.Status >= Enums.OrderStatus.Placed).OrderBy(c => c.PlacedDate).ToList())
            {
                // Search on the array if there is already a entry with that date
                var match = dailySells.Find(s => s.Date == cart.PlacedDate);
                // if there is no match then add a new entry
                if(match is null) {
                    dailySells.Add(new DailySells { Date = cart.PlacedDate, Amount = 1 });
                    continue;
                }
                // Update the amount of times this daily sell happened, this will also update the value on the list
                match.Amount += 1;
            }
            ViewBag.DailySells = dailySells;

            // Top five sellers
            List<TopFiveSellers> topFiveSellers = [];
            foreach (var cart in context.Cart.Where(c => c.Status >= Enums.OrderStatus.Placed).ToList())
            {
                var match = topFiveSellers.Find(s => s.Restaurant == cart.Restaurant);
                if (match is null)
                {
                    topFiveSellers.Add(new TopFiveSellers { Restaurant = cart.Restaurant, Sales = 1 });
                    continue;
                }
                match.Sales += 1;
            }
            ViewBag.TopFiveSellers = topFiveSellers.OrderByDescending(s => s.Sales).Take(5).ToList();

            // Top ten items
            List<TopTenBestSeller> topTenBestSeller = [];
            foreach (var cart in context.Cart.Where(c => c.Status >= Enums.OrderStatus.Placed).Include(c => c.Items).ToList())
            {
                foreach (var item in cart.Items ?? [])
                {
                    var match = topTenBestSeller.Find(s => s.Restaurant == cart.Restaurant && s.Item == item.Name);
                    if (match is null)
                    {
                        topTenBestSeller.Add(new TopTenBestSeller { Restaurant = cart.Restaurant, Item = item.Name, Sales = 1 });
                        continue;
                    }
                    match.Sales += 1;
                }
            }
            ViewBag.TopTenBestSeller = topTenBestSeller.OrderByDescending(s => s.Sales).Take(10).ToList();

            return View();
        }
    }
}
