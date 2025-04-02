using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    [Authorize]
    public class MyOrdersController(AeroBitesContext context) : Controller
    {
        private IQueryable<Cart>? Orders => context.Cart.Include(c => c.Items).Where(c => c.AccountId == User.GetId());

        /// <summary>
        /// Obtem a lista de pedidos que o cliente realizou. 
        /// Os pedidos retornados são todos que o cliente pagou por até aos que 
        /// estão à espera de serem recolhidos
        /// </summary>
        public async Task<IActionResult> Index() {
            if (Orders == null) { return View(new List<Cart>());}

            var orders = await Orders.Where(c => c.Status >= Enums.OrderStatus.Placed && c.Status <= Enums.OrderStatus.Waiting).OrderByDescending(c => c.Status).ToListAsync();

            return View(orders);
        }

        /// <summary>
        /// Obtém a lista de pedidos que já foram entregues, ou seja, no estado "Recieved".
        /// </summary>
        /// <returns>
        /// Retorna uma view com a lista de pedidos ou a mesma view mas com uma lista vazia caso não existam pedidos nessas condições.
        /// </returns>
        public async Task<IActionResult> History() {
            if(Orders == null) { return View(new List<Cart>()); }

            var orders = await Orders.Where(c => c.Status >= Enums.OrderStatus.Recieved).OrderByDescending(c => c.Id).ToListAsync();

            return View(orders);
        }

        /// <summary>
        /// Endpoint para o cliente indicar que recebeu o seu pedido
        /// </summary>
        /// <param name="orderId">Id do pedido</param>
        [HttpPost]
        public async Task<IActionResult> RecieveOrder(int orderId) {
            if (Orders == null) { return RedirectToAction(nameof(Index)); }

            var orders = await Orders.ToListAsync();
            var thisOrder = orders.Find(o => o.Id == orderId);

            if( thisOrder is null ) return RedirectToAction(nameof(Index));
            if( thisOrder.Status != Enums.OrderStatus.Waiting ) return RedirectToAction(nameof(Index));

            thisOrder.Status = Enums.OrderStatus.Recieved;
            context.Cart.Update(thisOrder);
            await context.SaveChangesAsync();

            TempData[Enums.MessageType.successMessage.ToString()] = "O estado do pedido foi alterado para recebido.";

            return RedirectToAction(nameof(Index));
        }
    }
}
