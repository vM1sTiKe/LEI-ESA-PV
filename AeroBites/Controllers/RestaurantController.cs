using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AeroBites.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AeroBites.Controllers
{
    [Authorize]
    public class RestaurantController(AeroBitesContext context) : Controller
    {
        /// <summary>
        /// Mostra a lista de restaurantes válidos e permite ao utilizador selecionar uma morada ativa.
        /// Carrega todas as moradas associadas ao utilizador e destaca a morada ativa na lista.
        /// </summary>
        /// <returns>View com a lista de restaurantes e as moradas disponíveis para o utilizador.</returns>
        public IActionResult Index() {
            var userId = User.GetId();

            var addresses = context.Address.Where(a => a.AccountId == userId).ToList();

            var activeAddressId = addresses.FirstOrDefault(a => a.IsActive)?.Id;

            ViewBag.Addresses = new SelectList(addresses, "Id", "FullAddress", activeAddressId);
            ViewBag.ActiveAddressId = activeAddressId;

            var restaurants = context.Restaurant.Where(restaurant => restaurant.Status == Enums.RestaurantStatus.Valid).ToList();
            return View(restaurants);
        }

        /// <summary>
        /// Exibe o menu de um restaurante específico, incluindo suas categorias e itens. 
        /// Também carrega o carrinho de compras atual do utilizador, caso exista.
        /// </summary>
        /// <param name="id">ID do restaurante cujo menu será exibido.</param>
        /// <returns>View com o menu do restaurante e o carrinho de compras do utilizador.</returns>
        public IActionResult Menu(int id)
        {
            var restaurant = context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.Id == id);
            ViewBag.Cart = context.Cart.Include(c => c.Items).Where(c => c.AccountId == User.GetId() && c.Status == Enums.OrderStatus.Choosing).FirstOrDefault();
            return View(restaurant);
        }

        /// <summary>
        /// Exibe o formulário de edição para um restaurante específico.
        /// Caso o restaurante não seja encontrado, retorna um erro 404 (NotFound).
        /// </summary>
        /// <param name="id">ID do restaurante a ser editado.</param>
        /// <returns>View com os dados do restaurante para edição ou erro 404 caso não exista.</returns>
        public IActionResult Edit(int id)
        {
            var restaurant = context.Restaurant.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

    }
}