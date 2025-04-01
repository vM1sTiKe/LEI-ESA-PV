using AeroBites.Data;
using AeroBites.Migrations;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    public class CheckoutController(AeroBitesContext context) : Controller
    {
        /// <summary>
        /// Retorna a página do checkout com toda a informação do pedido
        /// </summary>
        public IActionResult Index()
        {
            var userId = User.GetId();

            var cart = context.Cart
                .FirstOrDefault(c => c.AccountId == userId && c.Status == Enums.OrderStatus.Choosing);

            if(cart == null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "O carrinho está vazio.";
                return RedirectToAction("Index", "Restaurant");
            }

            cart.Items = context.CartItem.Where(i => i.CartId == cart.Id).ToList();

            ViewBag.RestaurantId = cart.RestaurantId;

            return View(cart);
        }

        /// <summary>
        /// Método responsável por realizar o checkout de um carrinho de compras.
        /// Altera o estado do carrinho de "Choosing" para "Placed" quando o utilizador confirma a escolha.
        /// Caso o carrinho não seja encontrado ou não esteja no estado "Choosing", redireciona para a pagina dos restaurantes.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendOrder(int cartId, int restaurantId)
        {
            var cart = await context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cartId && c.AccountId == User.GetId() && c.Status == Enums.OrderStatus.Choosing);

            if (cart == null) {
                return RedirectToAction("Index", "Restaurant");
            }

            float total_price = 0f;
            // For everyitem go calculate the total price of the order
            foreach(var item in cart.Items ?? []){
                total_price += item.Price;
            }

            cart.Status = Enums.OrderStatus.Placed;
            cart.TotalPrice = total_price;
            cart.PlacedDate = DateOnly.FromDateTime(DateTime.Now);
            
            var activeAddress = await context.Address.Where(address => address.AccountId == User.GetId() && address.IsActive).FirstOrDefaultAsync();
            var cartAddress = await context.CartAddress.Where(address => address.Latitude == activeAddress.Latitude && address.Longitude == activeAddress.Longitude).FirstOrDefaultAsync();
            
            if(cartAddress == null) 
            {
                cartAddress = new CartAddress
                {
                    Latitude = activeAddress.Latitude,
                    Longitude = activeAddress.Longitude,
                    FullAddress = activeAddress.FullAddress
                };
               
                context.CartAddress.Add(cartAddress);
                await context.SaveChangesAsync();
            }

            cart.CartAddressId = cartAddress.Id;

            context.Cart.Update(cart);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Preparing));
        }

        /// <summary>
        /// Retorna página a indicar ao cliente que o pedido está a ser preparado
        /// </summary>
        public IActionResult Preparing()
        {
            return View();
        }
    }
}
