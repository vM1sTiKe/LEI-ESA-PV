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

        private Address AddAddress(double lat, double lng, string fullAddress)
        {
            return new Address { Latitude = lat, Longitude = lng, FullAddress = fullAddress, IsActive = false };
        }

        public async Task<Address> AddAddressRestaurant(double lat, double lng, string fullAddress, int id)
        {
            // Desativa moradas anteriores do restaurante
            var existingAddresses = await _context.Address
                .Where(a => a.RestaurantId == id && a.IsActive)
                .ToListAsync();

            foreach (var addr in existingAddresses)
            {
                addr.IsActive = false;
            }

            var address = new Address
            {
                Latitude = lat,
                Longitude = lng,
                FullAddress = fullAddress,
                RestaurantId = id,
                IsActive = true
            };

            _context.Add(address);
            await _context.SaveChangesAsync();

            return address;
        }

        public async Task<Address> AddAddressAccount(double lat, double lng, string fullAddress, int id)
        {
            var address = this.AddAddress(lat, lng, fullAddress);

            address.IsActive = await _context.Address.AnyAsync(address => address.AccountId == id) ? false : true;
            address.AccountId = id;

            _context.Add(address);
            await _context.SaveChangesAsync();

            return address;
        }
    }
}
