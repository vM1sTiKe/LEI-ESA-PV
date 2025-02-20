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

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="context">The context to interact with the database. This is injected by the dependency injection container.</param>
        public AdminController(AeroBitesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of restaurants pending approval, as well as already approved ones for management actions.
        /// </summary>
        /// <returns>
        /// A view displaying the list of restaurants for approval or deletion.
        /// </returns>
        public IActionResult Restaurants()
        {
            return View(_context.Restaurant.ToList());
        }

        /// <summary>
        /// Displays a list of all available delivery points.
        /// </summary>
        /// <returns>
        /// A view showing all delivery points.
        /// </returns>
        public IActionResult Collections()
        {
            return View();
        }

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
            var restaurant = _context.Restaurant.Find(id);

            restaurant.Status = Enums.RestaurantStatus.Valid;
            _context.SaveChanges();

            TempData["RequestMessage"] = "Restaurante aprovado!";

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
            var restaurant = _context.Restaurant.Find(id);

            _context.Restaurant.Remove(restaurant);
            _context.SaveChanges();

            TempData["RequestMessage"] = "Restaurante negado!";

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
            var restaurant = _context.Restaurant.FirstOrDefault(restaurant => restaurant.Id == id);

            _context.Restaurant.Remove(restaurant);
            _context.SaveChanges();

            TempData["RequestMessage"] = "Restaurante eliminado!";

            return RedirectToAction(nameof(Index));
        }
    }
}
