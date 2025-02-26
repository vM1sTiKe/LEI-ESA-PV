using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    [Authorize]
    public class CategoryController(AeroBitesContext context) : Controller
    {
        private Restaurant? MyRestaurant => context.Restaurant.Include(r => r.Categories).FirstOrDefault(r => r.OwnerId == User.GetId());

        /// <summary>
        /// Displays the category creation page. If the restaurant is not set, redirects to create a restaurant first.
        /// </summary>
        /// <returns>
        /// The category creation view or a redirect to restaurant creation.
        /// </returns>
        public IActionResult Create() {
            if (MyRestaurant is null) return RedirectToAction("Create", "MyRestaurant");
            return View();
        }

        /// <summary>
        /// Handles category creation for the current restaurant.
        /// </summary>
        /// <param name="category">The category to be created.</param>
        /// <returns>
        /// A redirect to the category list or a BadRequest if the restaurant is invalid.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] Category category) {
            if( MyRestaurant is null ) return BadRequest();

            category.RestaurantId = MyRestaurant.Id;
            context.Category.Add(category);
            await context.SaveChangesAsync();
            return RedirectToAction("Categories", "MyRestaurant");
        }

        /// <summary>
        /// Deletes a category based on the given ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>
        /// A redirect to the category list or a BadRequest if validation fails.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)  {
            if (MyRestaurant is null) return BadRequest();
            if (id is null) return BadRequest();

            Category? category = MyRestaurant.Categories?.Find(c => c.Id == id);
            if (category is null) return BadRequest();

            context.Category.Remove(category);
            await context.SaveChangesAsync();
            return RedirectToAction("Categories", "MyRestaurant");
        }

        /// <summary>
        /// Displays the edit page for a specific category.
        /// </summary>
        /// <param name="id">The ID of the category to edit.</param>
        /// <returns>
        /// A view for editing the category or a redirect if validation fails.
        /// </returns>
        public IActionResult Edit(int? id) {
            if (MyRestaurant is null) return RedirectToAction("Create", "MyRestaurant");
            if (id is null) return BadRequest();

            Category? category = MyRestaurant.Categories?.Find(c => c.Id == id);
            if (category is null) return BadRequest();

            return View(category);
        }

        /// <summary>
        /// Handles category updates.
        /// </summary>
        /// <param name="c">The category object containing updated data.</param>
        /// <returns>
        /// A redirect to the category list or a BadRequest if validation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id", "Name")] Category c) {
            if (MyRestaurant is null) return BadRequest();

            Category? category = MyRestaurant.Categories?.Find(x => x.Id == c.Id);
            if (category is null) return BadRequest();

            category.Name = c.Name;

            context.Category.Update(category);
            await context.SaveChangesAsync();
            return RedirectToAction("Categories", "MyRestaurant");
        }
    }
}
