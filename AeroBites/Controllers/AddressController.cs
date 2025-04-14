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

        /// <summary>
        /// Ação que retorna a lista de moradas do utilizador autenticado, ordenadas por estado ativo e ID.
        /// </summary>
        /// <returns>Uma view com a lista de moradas do utilizador.</returns>
        public async Task<IActionResult> Index()
        {
            var userId = User.GetId();
            var addresses = await _context.Address
                .Where(a => a.AccountId == userId)
                .OrderByDescending(a => a.IsActive)
                .ThenBy(a => a.Id)
                .ToListAsync();

            return View(addresses);
        }

        /// <summary>
        /// Adiciona uma nova morada para o utilizador autenticado com base nas coordenadas e endereço fornecidos.
        /// </summary>
        /// <param name="lat">Latitude da morada.</param>
        /// <param name="lng">Longitude da morada.</param>
        /// <param name="fullAddress">Endereço completo da morada.</param>
        /// <returns>Redireciona para a página Index após adicionar a morada.</returns>
        [HttpPost]
        public async Task<IActionResult> AddAddress(double lat, double lng, string fullAddress)
        {
            var userId = User.GetId();
            await _addressService.AddAddressAccount(lat, lng, fullAddress, userId);

            TempData[Enums.MessageType.successMessage.ToString()] = "Morada adicionada.";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Remove uma morada do utilizador autenticado com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da morada a remover.</param>
        /// <returns>Redireciona para a página Index após remover a morada, ou NotFound se a morada não existir ou não pertencer ao utilizador.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveAddress(int id)
        {
            var userId = User.GetId();  
            var address = await _context.Address.FindAsync(id);

            if (address == null || address.AccountId != userId)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Impossivel remover morada.";

                return RedirectToAction("Index");
            }

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            TempData[Enums.MessageType.successMessage.ToString()] = "Morada eliminada.";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Define uma morada como ativa para o utilizador autenticado, desativando a anterior se existir.
        /// </summary>
        /// <param name="id">ID da nova morada a definir como ativa.</param>
        /// <returns>Retorna OK se a operação for bem-sucedida, ou NotFound se a morada não existir ou não pertencer ao utilizador.</returns>
        [HttpPost]
        public async Task<IActionResult> SelectAddress(int id)
        {
            var oldAddress = await _context.Address.FirstOrDefaultAsync(address => address.IsActive == true && address.AccountId == User.GetId());
            if(oldAddress != null)
            {
                oldAddress.IsActive = false;
            }

            var newAddress = await _context.Address.FindAsync(id);
            if(newAddress == null || newAddress.AccountId != User.GetId())
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Impossivel selecionar morada.";

                return RedirectToAction("Index");
            }
            
            newAddress.IsActive = true;

            await _context.SaveChangesAsync();

            TempData[Enums.MessageType.successMessage.ToString()] = "Morada Selecionada.";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtém todas as moradas do utilizador ordenadas pelo estado ativo.
        /// </summary>
        /// <returns>Retorna a lista de moradas do utilizador.</returns>
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
