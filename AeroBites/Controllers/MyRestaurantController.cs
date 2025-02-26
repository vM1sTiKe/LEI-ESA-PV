using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    [Authorize]
    public class MyRestaurantController(AeroBitesContext context) : Controller
    {
        private Restaurant? MyRestaurant => context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.OwnerId == User.GetId());

        /// <summary>
        /// Displays the restaurant's home page or redirects to the create page if no restaurant exists.
        /// Redirects to the Reviewing page if the restaurant is still waiting for acceptance.
        /// </summary>
        /// <returns>
        /// The restaurant's homepage view or the create page.
        /// </returns>
        public IActionResult Index() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status == Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Reviewing));
            return View(MyRestaurant);
        }

        /// <summary>
        /// Displays the restaurant creation page if no restaurant exists.
        /// Redirects to the index if a restaurant is already created.
        /// </summary>
        /// <returns>
        /// The restaurant creation view or redirect to the index.
        /// </returns>
        public IActionResult Create() {
            return MyRestaurant is null ? View() : RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Handles the creation of a new restaurant.
        /// </summary>
        /// <param name="restaurant">The new restaurant data to be created.</param>
        /// <returns>
        /// A redirect to the Reviewing page after creation.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] Restaurant restaurant) {
            restaurant.OwnerId = User.GetId();
            context.Add(restaurant);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Reviewing));
        }

        /// <summary>
        /// Displays the reviewing page for a restaurant that is waiting for acceptance.
        /// Redirects to the index page if the restaurant is already accepted.
        /// </summary>
        /// <returns>
        /// The restaurant reviewing view or redirect to the index.
        /// </returns>
        public IActionResult Reviewing() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status != Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Index));
            return View();
        }

        /// <summary>
        /// Handles the editing of a restaurant's details.
        /// </summary>
        /// <param name="restaurant">The restaurant data to be updated.</param>
        /// <returns>
        /// A redirect to the index page after editing.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Name")] Restaurant restaurant) {
            if( MyRestaurant is null ) return RedirectToAction(nameof(Index));

            MyRestaurant.Name = restaurant.Name;
            context.Update(MyRestaurant);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays a list of all items in the restaurant, including those from all categories.
        /// </summary>
        /// <returns>
        /// A view displaying the restaurant's items.
        /// </returns>
        public IActionResult Items() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));

            List<Item> items = [];
            foreach (Category category in MyRestaurant.Categories ?? []) {
                items.AddRange(category.Items ?? []);
            }
            return View(items);
        }

        /// <summary>
        /// Displays the list of categories in the restaurant.
        /// </summary>
        /// <returns>
        /// A view displaying the restaurant's categories.
        /// </returns>
        public IActionResult Categories() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));
            return View(MyRestaurant.Categories);
        }

        /// <summary>
        /// Displays the orders page for the restaurant.
        /// </summary>
        /// <returns>
        /// The orders view for the restaurant.
        /// </returns>
        public IActionResult Orders() {
            return View();
        }
    }
}
