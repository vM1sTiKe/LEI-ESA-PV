using AeroBites.Data;
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
        public async Task<IActionResult> Preparing(int cartId, int restaurantId)
        {
            var cart = await context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cartId && c.AccountId == User.GetId() && c.Status == Enums.OrderStatus.Choosing);

            if (cart == null) {
                return RedirectToAction("Index", "Restaurant");
            }

            cart.Status = Enums.OrderStatus.Placed;
            cart.PlacedDate = DateOnly.FromDateTime(DateTime.Now);
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
