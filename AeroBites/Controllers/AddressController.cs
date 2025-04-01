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
        public async Task<IActionResult> AddAddress(double lat, double lng, string fullAddress)
        {
            var userId = User.GetId();

            if(userId == null)
            {
                return Unauthorized();
            }
            
            var address = await _addressService.AddAddress(lat, lng, fullAddress, userId);

            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAddress(int id)
        {
            var userId = User.GetId();  
            var address = await _context.Address.FindAsync(id);

            if (address == null || address.AccountId != userId)
            {
                return NotFound();
            }

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SelectAddress(int id)
        {
            var userId = User.GetId();

            var oldAddress = await _context.Address.FirstOrDefaultAsync(address => address.isActive && address.AccountId == userId);
            if (oldAddress != null)
            {
                oldAddress.isActive = false; // Desativa a antiga morada
            }

            var newAddress = await _context.Address.FindAsync(id);
            if (newAddress == null || newAddress.AccountId != userId)
            {
                return NotFound();
            }

            newAddress.isActive = true; // Ativa a nova morada

            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> GetAllAddresses()
        {
            var userId = User.GetId();
            var addressList = await _context.Address
                .Where(address => address.AccountId == userId)
                .OrderByDescending(address => address.isActive)
                .ThenBy(address => address.Id)
                .ToListAsync();

            return Ok(addressList);
        }
    }
}
