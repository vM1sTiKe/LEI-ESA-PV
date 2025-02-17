using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class ItemController(AeroBitesContext context) : Controller
    {
        private Restaurant? MyRestaurant => context.Restaurant.FirstOrDefault(r => r.OwnerId == User.GetId());

        public async Task<IActionResult> Create([Bind("Name")] Item item) {
            if (MyRestaurant is null) return BadRequest();

            item.RestaurantId = MyRestaurant.Id;
            context.Add(item);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
