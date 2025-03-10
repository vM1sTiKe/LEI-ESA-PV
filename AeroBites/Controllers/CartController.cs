using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

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
        /// <param name="item">O item que será adicionado ao carrinho.</param>
        /// <param name="restaurant">O restaurante ao qual o item pertence.</param>
        /// <returns>Retorna a View do Menu.</returns>
        [HttpPost]
        public async Task<IActionResult> AddItem(int item, int restaurant)
        {
            var restaurante = await _context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefaultAsync(r => r.Id == restaurant);

            if (restaurante is null)
            {
                return RedirectToAction("Menu", "Restaurant", new { id = restaurant });
            }

            

            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(cart => cart.AccountId == User.GetId());
            System.Diagnostics.Debug.WriteLine("A");
            if (cart == null)
            {
                System.Diagnostics.Debug.WriteLine("B");
                cart = CreateNewCart(restaurante.Name);
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }
            else if (cart != null && cart.Restaurant != restaurante.Name)
            {
                System.Diagnostics.Debug.WriteLine("C");
                _context.Cart.Remove(cart);

                cart = CreateNewCart(restaurante.Name);
                _context.Cart.Add(cart);

                await _context.SaveChangesAsync();
            }

            System.Diagnostics.Debug.WriteLine("D");

            CartItem cartItem = null;

            foreach (var category in restaurante.Categories ?? [])
            {
                foreach (var catItem in category.Items ?? [])
                {
                    if (catItem.Id == item)
                    {
                        cartItem = new CartItem { Name = catItem.Name, Price = catItem.Price, CartId = cart.Id };
                    }
                }
            }

            if (cartItem is null)
            {
                return RedirectToAction("Menu", "Restaurant", new { id = restaurant });
            }

            cart.Items.Add(cartItem);
            await _context.SaveChangesAsync();

            TempData["RequestMessage"] = "Item adicionado.";
            
            return RedirectToAction("Menu", "Restaurant", new { id=restaurant});
        }


        /// <summary>
        /// Remove um item do carrinho do utilizador. 
        /// </summary>
        /// <param name="item">O item que será removido do carrinho.</param>
        /// <param name="restaurant">O restaurante ao qual o item pertence.</param>
        /// <returns>Retorna a View do Menu.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveItem (int item, int restaurant)
        {
            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(cart => cart.AccountId == User.GetId());
            var cartItem = await _context.CartItem.FirstOrDefaultAsync(i => i.Id == item);

            if (cart != null && cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Menu", "Restaurant", new { id = restaurant });
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