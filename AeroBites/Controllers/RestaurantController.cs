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

        /// <summary>
        /// Initializes a new instance of the <see cref="RestaurantController"/> class.
        /// </summary>
        /// <param name="context">The database context used to access restaurant-related data.</param>
        public RestaurantController(AeroBitesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of valid restaurants that are currently available.
        /// Filters out restaurants that are not validated yet.
        /// </summary>
        /// <returns>
        /// A view with a list of valid restaurants.
        /// </returns>
        public IActionResult Index() {
            var restaurants = _context.Restaurant.Where(restaurant => restaurant.Status == Enums.RestaurantStatus.Valid).ToList();
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
            var restaurant = _context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.Id == id);
            return View(restaurant);
        }
    }
}