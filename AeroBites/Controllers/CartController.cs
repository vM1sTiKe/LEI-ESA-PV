using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    public class CartController : Controller
    {
        private readonly AeroBitesContext _context;

        public CartController(AeroBitesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona um item ao carrinho do utilizador. 
        /// Se o carrinho não existir ou se o restaurante for diferente do atual, um novo é criado.
        /// </summary>
        /// <param name="cartItem">O item que será adicionado ao carrinho.</param>
        /// <param name="restaurant">O restaurante ao qual o item pertence.</param>
        /// <returns>Retorna um status HTTP Ok em caso de sucesso ou BadRequest se o nome do restaurante for inválido.</returns>
        [HttpPost]
        public async Task<IActionResult> AddItem([Bind("Name", "Price")] CartItem cartItem, [Bind("Name")] Restaurant restaurant)
        {
            if (string.IsNullOrEmpty(restaurant?.Name))
            {
                return BadRequest("O nome do restaurante é obrigatório.");
            }

            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(cart => cart.AccountId == User.GetId());
            System.Diagnostics.Debug.WriteLine("A");
            if (cart == null)
            {
                System.Diagnostics.Debug.WriteLine("B");
                cart = CreateNewCart(restaurant.Name);
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }
            else if (cart != null && cart.Restaurant != restaurant.Name)
            {
                System.Diagnostics.Debug.WriteLine("C");
                _context.Cart.Remove(cart);

                cart = CreateNewCart(restaurant.Name);
                _context.Cart.Add(cart);

                await _context.SaveChangesAsync();
            }

            System.Diagnostics.Debug.WriteLine("D");
            cartItem.CartId = cart.Id;
            cart.Items.Add(cartItem);
            await _context.SaveChangesAsync();

            TempData["RequestMessage"] = "Item adicionado.";
            
            return Ok();
        }

        /// <summary>
        /// Apaga o carrinho.
        /// </summary>
        /// <returns>Retorna a view de listagem dos restaurantes.</returns>
        public async Task<IActionResult> DeleteCart()
        {
            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(cart => cart.AccountId == User.GetId());
            if (cart != null)
            {
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();
                TempData["RequestMessage"] = "Carrinho eliminado.";
            }
            
            return RedirectToAction("Index", "Restaurant");
        }

        /// <summary>
        /// Cria um novo carrinho para o utilizador, associando-o a um restaurante específico.
        /// </summary>
        /// <param name="restaurantName">O nome do restaurante que será associado ao novo carrinho.</param>
        /// <returns>Retorna o objeto `Cart` criado, pronto para ser salvo no banco de dados.</returns>
        private Cart CreateNewCart(string restaurantName )
        {
            var cart = new Cart
            {
                Status = Enums.OrderStatus.Choosing,
                Restaurant = restaurantName,
                Address = "Something", //To be changed
                Items = new List<CartItem>(),
                AccountId = User.GetId()
            };

            return cart;
        }
    }
}