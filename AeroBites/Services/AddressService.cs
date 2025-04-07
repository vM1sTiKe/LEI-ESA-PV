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

        /// <summary>
        /// Cria e retorna um novo objeto de morada (Address) com os dados fornecidos.
        /// A morada criada será inicialmente marcada como inativa.
        /// </summary>
        /// <param name="lat">Latitude da morada.</param>
        /// <param name="lng">Longitude da morada.</param>
        /// <param name="fullAddress">Endereço completo da morada.</param>
        /// <returns>Um novo objeto Address com os dados fornecidos e a propriedade IsActive definida como false.</returns>
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

        /// <summary>
        /// Adiciona uma nova morada ativa para o restaurante especificado e desativa as moradas anteriores.
        /// </summary>
        /// <param name="lat">Latitude da nova morada.</param>
        /// <param name="lng">Longitude da nova morada.</param>
        /// <param name="fullAddress">Nova morada.</param>
        /// <param name="id">ID do restaurante ao qual a morada será associada.</param>
        /// <returns>O objeto Address com os dados da nova morada criada.</returns>
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
