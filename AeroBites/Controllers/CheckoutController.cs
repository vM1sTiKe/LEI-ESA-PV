using System.Text.Json.Nodes;
using AeroBites.Data;
using AeroBites.Models;
using AeroBites.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    [Authorize]
    public class CheckoutController(AeroBitesContext context, IConfiguration config) : Controller
    {
        private IQueryable<PaymentMethod> MyMethods => context.PaymentMethod.Where(m => m.AccountId == User.GetId());

        /// <summary>
        /// Retorna a página do checkout com toda a informação do pedido
        /// </summary>
        public IActionResult Index(int restaurant)
        {
            var userId = User.GetId();

            var cart = context.Cart
                .FirstOrDefault(c => c.AccountId == userId && c.Status == Enums.OrderStatus.Choosing);

            if (cart == null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "O carrinho está vazio.";
                return RedirectToAction("Menu", "Restaurant", new { id = restaurant });
            }

            cart.Items = context.CartItem.Where(i => i.CartId == cart.Id).ToList();

            ViewBag.RestaurantId = cart.RestaurantId;
            ViewBag.PaymentMethods = new SelectList(MyMethods.ToList(), "Id", "Details", MyMethods.Where(m => m.IsDefault == true).FirstOrDefault()?.Id);

            var activeAddress = context.Address.FirstOrDefault(a => a.AccountId == userId && a.IsActive);

            ViewBag.ActiveAddress = activeAddress;

            return View(cart);
        }

        /// <summary>
        /// Método responsável por realizar o checkout de um carrinho de compras.
        /// Altera o estado do carrinho de "Choosing" para "Placed" quando o utilizador confirma a escolha.
        /// Caso o carrinho não seja encontrado ou não esteja no estado "Choosing", redireciona para a pagina dos restaurantes.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendOrder(int cartId, int restaurantId, int paymentMethod)
        {
            // Recolhe, da lista de métodos de pagamento do utilizador, o método enviado
            var payment_method = MyMethods.Where(m => m.Id == paymentMethod).FirstOrDefault();
            // Não pode criar pedido se o método enviado não for do utilizador / não for válido
            if (payment_method is null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Nenhum método de pagamento válido encontrado.";
                return RedirectToAction("Menu", "Restaurant", new { id = restaurantId });
            }

            var cart = await context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cartId && c.AccountId == User.GetId() && c.Status == Enums.OrderStatus.Choosing);

            if (cart == null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "O carrinho está vazio.";
                return RedirectToAction("Menu", "Restaurant", new { id = restaurantId });
            }

            var activeAddress = await context.Address.Where(address => address.AccountId == User.GetId() && address.IsActive).FirstOrDefaultAsync();
            // Se não existe endereço ativo não pode finalizar o checkout
            if (activeAddress is null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Nenhuma morada de entrega válida encontrada.";
                return RedirectToAction("Menu", "Restaurant", new { id = restaurantId });
            }

            float total_price = 0f;
            // For everyitem go calculate the total price of the order
            foreach (var item in cart.Items ?? [])
            {
                total_price += item.Price;
            }

            JsonNode response = await new PayPalService(config).Pay(payment_method.ApiToken, total_price);
            if (response["status"]?.ToString() != "COMPLETED")
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Não foi possível realizar o pagamento.";
                return RedirectToAction("Menu", "Restaurant", new { id = restaurantId });
            }

            cart.Status = Enums.OrderStatus.Placed;
            cart.TotalPrice = total_price;
            cart.PlacedDate = DateOnly.FromDateTime(DateTime.Now);

           
            var cartAddress = await context.CartAddress.Where(address => address.Latitude == activeAddress.Latitude && address.Longitude == activeAddress.Longitude).FirstOrDefaultAsync();

            if (cartAddress == null)
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

            TempData[Enums.MessageType.successMessage.ToString()] = "Pedido submetido e pago com sucesso.";

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
