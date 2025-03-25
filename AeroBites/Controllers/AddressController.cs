using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class AddressController : Controller
    {
        private AeroBitesContext _context;

        public AddressController(AeroBitesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(double lat, double lng, string fullAddress)
        {
            var address = new Address
            {
                Latitude = lat,
                Longitude = lng,
                FullAddress = fullAddress,
                AccountId = User.GetId()
            };

            _context.Add(address);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
