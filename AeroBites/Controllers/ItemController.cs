using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    public class ItemController(AeroBitesContext context) : Controller
    {
        private Restaurant? MyRestaurant => context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.OwnerId == User.GetId());

        /// <summary>
        /// Displays the item creation form. If no restaurant exists, redirects to create a restaurant first.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create() {
            if (MyRestaurant is null) return RedirectToAction("Create", "MyRestaurant");

            ViewBag.Categories = new SelectList(MyRestaurant.Categories, "Id", "Name");
            return View();
        }

        /// <summary>
        /// Handles the creation of a new item.
        /// </summary>
        /// <param name="item">The item to be created.</param>
        /// <returns>
        /// A redirect to the item list view or a BadRequest if validation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name", "Price", "CategoryId")] Item item) {
            if (MyRestaurant is null) return BadRequest();

            context.Item.Add(item);
            await context.SaveChangesAsync();
            return RedirectToAction("Items", "MyRestaurant");
        }

        /// <summary>
        /// Deletes an item by its ID.
        /// </summary>
        /// <param name="id">The ID of the item to delete.</param>
        /// <returns>
        /// A redirect to the item list view or a BadRequest if validation fails.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id) {
            if (MyRestaurant is null) return BadRequest();
            if (id is null) return BadRequest();

            Item? item = GetMyItem(id);
            if (item is null) return BadRequest();

            context.Item.Remove(item);
            await context.SaveChangesAsync();
            return RedirectToAction("Items", "MyRestaurant");
        }

        /// <summary>
        /// Displays the edit form for an item.
        /// </summary>
        /// <param name="id">The ID of the item to edit.</param>
        /// <returns>
        /// The item edit view or a BadRequest if validation fails.
        /// </returns>
        public IActionResult Edit(int? id) {
            if (MyRestaurant is null) return RedirectToAction("Create", "MyRestaurant");
            if (id is null) return BadRequest();

            Item? item = GetMyItem(id);
            if (item is null) return BadRequest();

            ViewBag.Categories = new SelectList(MyRestaurant.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        /// <summary>
        /// Handles updating an existing item.
        /// </summary>
        /// <param name="i">The updated item data.</param>
        /// <returns>
        /// A redirect to the item list view or a BadRequest if validation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id", "Name", "Price", "CategoryId")] Item i) {
            if (MyRestaurant is null) return BadRequest();

            Item? item = GetMyItem(i.Id);
            if (item is null) return BadRequest();

            item.Name = i.Name;
            item.Price = i.Price;
            item.CategoryId = i.CategoryId;

            context.Item.Update(item);
            await context.SaveChangesAsync();
            return RedirectToAction("Items", "MyRestaurant");
        }

        /// <summary>
        /// Retrieves an item by ID for the current restaurant's categories.
        /// </summary>
        /// <param name="id">The ID of the item to retrieve.</param>
        /// <returns>
        /// The item if found, otherwise null.
        /// </returns>
        private Item? GetMyItem(int? id) {
            if (id is null) return null;
            if (MyRestaurant is null) return null;

            foreach (Category category in MyRestaurant.Categories ?? []) {
                foreach (Item item in category.Items ?? []) {
                    if (item.Id == id) return item;
                }
            }

            return null;
        }
    }
}
