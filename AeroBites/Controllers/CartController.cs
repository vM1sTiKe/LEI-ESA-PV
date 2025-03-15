using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    public class CartController(AeroBitesContext _context) : Controller
    {
        private Cart? MyCart => _context.Cart.Include(c => c.Items).FirstOrDefault(cart => cart.AccountId == User.GetId() && cart.Status == Enums.OrderStatus.Choosing);

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
            if (this.GetRestaurant(restaurant) is null) return RedirectToAction("Menu", "Restaurant", new { id = restaurant });

            // No current cart create new
            if (MyCart == null) {
                _context.Cart.Add(CreateNewCart(restaurant));
                await _context.SaveChangesAsync();
            }
            // There is a cart but its not from the current restaurant
            else if (MyCart != null && MyCart.RestaurantId != restaurant) {
                _context.Cart.Remove(MyCart); // Remove old
                _context.Cart.Add(CreateNewCart(restaurant)); //Create new on this restaurant
                await _context.SaveChangesAsync();
            }

            // Get from the items list the item we want to add
            Item i = this.GetItemsFromRestaurante(restaurant).Find(i => i.Id == item);
            if (i is null) return RedirectToAction("Menu", "Restaurant", new { id = restaurant });

            _context.CartItem.Add(new CartItem { Name = i.Name, Price = i.Price, CartId = MyCart.Id });
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Menu", "Restaurant", new { id= restaurant });
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
            var cartItem = await _context.CartItem.FirstOrDefaultAsync(i => i.Id == item);

            if (MyCart != null && cartItem != null) {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Menu", "Restaurant", new { id = restaurant });
        }

        /// <summary>
    /// Método responsável por realizar o checkout de um carrinho de compras.
    /// Altera o estado do carrinho de "Choosing" para "Placed" quando o utilizador confirma a escolha.
    /// Caso o carrinho não seja encontrado ou não esteja no estado "Choosing", redireciona para o menu do restaurante.
    /// </summary>
    /// <param name="cartId">ID do carrinho de compras que será alterado.</param>
    /// <param name="restaurantId">ID do restaurante para o qual o pedido será feito.</param>
    /// <returns>Redireciona para o menu do restaurante após o checkout ser realizado.</returns>
        [HttpPost]
        public async Task<IActionResult> Checkout(int cartId, int restaurantId)
        {
            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cartId && c.AccountId == User.GetId() && c.Status == Enums.OrderStatus.Choosing);

            if(cart == null)
            {
                return RedirectToAction("Menu", "Restaurant", new { id = restaurantId });
            }

            cart.Status = Enums.OrderStatus.Placed;
            await _context.SaveChangesAsync();

            return RedirectToAction("Menu", "Restaurant", new { id = restaurantId });
        }

        /// <summary>
        /// Método responsável por enviar o pedido de um carrinho de compras, alterando o estado de "Preparing" para "OnTheWay".
        /// Se o carrinho não for encontrado ou não estiver no estado "Preparing", o método retorna um erro.
        /// </summary>
        /// <param name="cartId">ID do carrinho de compras que será enviado.</param>
        /// <returns>Retorna um status OK se o pedido for enviado com sucesso ou um erro caso contrário.</returns>
        [HttpPost]
        public async Task<IActionResult> SendOrder(int cartId)
        {
            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cartId && c.AccountId == User.GetId() && c.Status == Enums.OrderStatus.Preparing);

            if (cart == null)
            {
                return BadRequest();
            }

            cart.Status = Enums.OrderStatus.OnTheWay;
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Apaga o carrinho.
        /// </summary>
        /// <returns>Retorna a view de listagem dos restaurantes.</returns>
        public async Task<IActionResult> DeleteCart()
        {
            if (MyCart != null) {
                _context.Cart.Remove(MyCart);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction("Index", "Restaurant");
        }

        /// <summary>
        /// Cria um novo carrinho para o utilizador, associando-o a um restaurante específico.
        /// Este método apenas pode ser chamado quando existe a garantia que o restaurante existe.
        /// </summary>
        /// <param name="restaurantId">O nome do restaurante que será associado ao novo carrinho.</param>
        /// <returns>Retorna o objeto `Cart` criado, pronto para ser salvo no banco de dados.</returns>
        private Cart CreateNewCart(int restaurantId)
        {
            var r = this.GetRestaurant(restaurantId) ?? throw new Exception("Invalid cart creation");

            return new Cart {
                Restaurant = r.Name,
                RestaurantId = r.Id,
                AccountId = User.GetId()
            };
        }

        /// <summary>
        /// Retorna o Restaurante que possui o id enviado
        /// </summary>
        private Restaurant? GetRestaurant(int restaurantId) {
            return _context.Restaurant.FirstOrDefault(r => r.Id == restaurantId) ?? null;
        }

        /// <summary>
        /// Retorna todos os items do restaurante
        /// </summary>
        private List<Item> GetItemsFromRestaurante(int restaurantId) {
            var r = _context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.Id == restaurantId) ?? null;
            if (r == null) return [];

            List<Item> items = [];
            foreach (Category c in r.Categories ?? []) {
                foreach (Item i in c.Items ?? []) items.Add(i);
            }
            return items;
        }
    }
}