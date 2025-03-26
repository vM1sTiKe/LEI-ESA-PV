using AeroBites.Data;
using AeroBites.Models;
using AeroBites.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    public class AddressController : Controller
    {
        private AeroBitesContext _context;
        private readonly AddressService _addressService;

        public AddressController(AeroBitesContext context, AddressService addressService)
        {
            _context = context;
            _addressService = addressService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(double lat, double lng, string fullAddress, int id)
        {
            var address = await _addressService.AddAddress(lat, lng, fullAddress, id);

            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAddress(int id)
        {
            var address = await _context.Address.FindAsync(id);
            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SelectAddress(int id)
        {
            var oldAddress = await _context.Address.FirstOrDefaultAsync(address => address.isActive == true && address.AccountId == User.GetId());
            oldAddress.isActive = false;

            var newAddress = await _context.Address.FindAsync(id);
            newAddress.isActive = true;

            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> GetAllAddresses()
        {
            var addressList = await _context.Address
                .Where(address => address.AccountId == User.GetId())
                .OrderByDescending(address => address.isActive == true)
                .ThenBy(address => address.Id)
                .ToListAsync();

            return Ok(addressList);
        }
    }
}
