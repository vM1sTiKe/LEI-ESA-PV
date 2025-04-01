using AeroBites.Data;
using AeroBites.Models;
using AeroBites.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    [Authorize]
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

        /// <summary>
        /// Adiciona um novo endereço à conta do utilizador.
        /// </summary>
        /// <param name="lat">Latitude do endereço.</param>
        /// <param name="lng">Longitude do endereço.</param>
        /// <param name="fullAddress">Endereço completo.</param>
        /// <param name="id">Identificador da conta.</param>
        /// <returns>Retorna o endereço adicionado.</returns>
        [HttpPost]
        public async Task<IActionResult> AddAddress(double lat, double lng, string fullAddress)
        {
            var address = await _addressService.AddAddressAccount(lat, lng, fullAddress, id);

            TempData[Enums.MessageType.successMessage.ToString()] = "Morada adicionada.";

            return Ok(address);
        }

        /// <summary>
        /// Remove um endereço com base no seu identificador.
        /// </summary>
        /// <param name="id">Identificador do endereço a remover.</param>
        /// <returns>Retorna uma resposta HTTP 200 se for bem-sucedido.</returns>
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

            TempData[Enums.MessageType.successMessage.ToString()] = "Morada eliminada.";

            return Ok();
        }

        /// <summary>
        /// Define um endereço como o endereço ativo do utilizador.
        /// </summary>
        /// <param name="id">Identificador do novo endereço ativo.</param>
        /// <returns>Retorna uma resposta HTTP 200 se for bem-sucedido.</returns>
        [HttpPost]
        public async Task<IActionResult> SelectAddress(int id)
        {
            var oldAddress = await _context.Address.FirstOrDefaultAsync(address => address.IsActive == true && address.AccountId == User.GetId());
            oldAddress.IsActive = false;

            var newAddress = await _context.Address.FindAsync(id);
            newAddress.IsActive = true;

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Obtém todos os endereços do utilizador ordenados pelo estado ativo.
        /// </summary>
        /// <returns>Retorna a lista de endereços do utilizador.</returns>
        public async Task<IActionResult> GetAllAddresses()
        {
            var userId = User.GetId();
            var addressList = await _context.Address
                .Where(address => address.AccountId == User.GetId())
                .OrderByDescending(address => address.IsActive == true)
                .ThenBy(address => address.Id)
                .ToListAsync();

            return Ok(addressList);
        }
    }
}
