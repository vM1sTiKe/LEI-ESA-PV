using AeroBites.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public List<Restaurant> GetValidRestaurants()
        {
            var validRestaurantes = _context.Restaurant.Where(restaurant => restaurant.Status == Enums.RestaurantStatus.Valid).ToList();
            return validRestaurantes;
        }
    }
}