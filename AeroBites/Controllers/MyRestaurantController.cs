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
        private Restaurant? MyRestaurant => context.Restaurant.Include(r => r.Categories).FirstOrDefault(r => r.OwnerId == User.GetId());

        public IActionResult Index() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status == Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Reviewing));
            return View(MyRestaurant);
        }

        public IActionResult Create() {
            return MyRestaurant is null ? View() : RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] Restaurant restaurant) {
            restaurant.OwnerId = User.GetId();
            context.Add(restaurant);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Reviewing));
        }

        public IActionResult Reviewing() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status != Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Index));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Name")] Restaurant restaurant) {
            if( MyRestaurant is null ) return RedirectToAction(nameof(Index));

            MyRestaurant.Name = restaurant.Name;
            context.Update(MyRestaurant);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Items() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));

            return View();
        }

        public IActionResult Categories() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));
            return View(MyRestaurant.Categories);
        }

        public IActionResult Orders() {
            return View();
        }
    }
}
