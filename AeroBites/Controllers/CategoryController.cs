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

        public IActionResult Create() {
            if (MyRestaurant is null) return RedirectToAction("Create", "MyRestaurant");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] Category category) {
            if( MyRestaurant is null ) return BadRequest();

            category.RestaurantId = MyRestaurant.Id;
            context.Category.Add(category);
            await context.SaveChangesAsync();
            return RedirectToAction("Categories", "MyRestaurant");
        }

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

        public IActionResult Edit(int? id) {
            if (MyRestaurant is null) return RedirectToAction("Create", "MyRestaurant");
            if (id is null) return BadRequest();

            Category? category = MyRestaurant.Categories?.Find(c => c.Id == id);
            if (category is null) return BadRequest();

            return View(category);
        }

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
