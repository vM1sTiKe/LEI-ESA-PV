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