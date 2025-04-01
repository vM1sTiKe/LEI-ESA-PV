using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Services
{
    public class AddressService
    {
        private readonly AeroBitesContext _context;

        public AddressService(AeroBitesContext context)
        {
            _context = context;
        }

        public async Task<Address> AddAddress(double lat, double lng, string fullAddress, int userId)
        {
            var address = new Address
            {
                Latitude = lat,
                Longitude = lng,
                FullAddress = fullAddress,
                AccountId = userId,
                isActive = false
            };

            _context.Add(address);
            await _context.SaveChangesAsync();

            return address;
        }
    }
}
