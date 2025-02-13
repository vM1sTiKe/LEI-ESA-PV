using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AeroBites.Data;

namespace AeroBites.Controllers
{
    [Authorize]
    public class RestaurantesController : Controller
    {
        private readonly AeroBitesContext _context;

        public RestaurantesController(AeroBitesContext context)
        {
            _context = context;
        }

        private Restaurant? MyRestaurant => _context.Restaurant.FirstOrDefault(r => r.OwnerId == User.GetId());

        public IActionResult Index() {
            return View();
        }
        
        public IActionResult Mine() {
            return MyRestaurant is null ? RedirectToAction(nameof(MineCreate)) : View("Mine/Index");
        }

        [Route("Restaurantes/Mine/Create")]
        public IActionResult MineCreate() {
            return MyRestaurant is not null ? RedirectToAction(nameof(Mine)) : View("Mine/Create");
        }

        [HttpPost]
        [Route("Restaurantes/Mine/Create")]
        public async Task<IActionResult> MineCreate([Bind("Name")] Restaurant restaurant)
        {
            restaurant.OwnerId = User.GetId();
            _context.Add(restaurant);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Mine));
        }

        [HttpGet]
        public List<Restaurant> GetValidRestaurants()
        {
            var validRestaurantes = _context.Restaurant.Where(restaurant => restaurant.Status == Enums.RestaurantStatus.Valid).ToList();
            return validRestaurantes;
        }

    }
}