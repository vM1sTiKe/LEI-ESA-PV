using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    public class MyOrders(AeroBitesContext context) : Controller
    {
        private IQueryable<Cart>? Orders => context.Cart.Where(c => c.AccountId == User.GetId());

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Obtém a lista de pedidos que já foram entregues, ou seja, no estado "Recieved".
        /// </summary>
        /// <returns>
        /// Retorna uma view com a lista de pedidos ou a mesma view mas com uma lista vazia caso não existam pedidos nessas condições.
        /// </returns>
        public async Task<IActionResult> History()
        {
            if(Orders == null) { return View(new List<Cart>()); }

            var orders = await Orders.Include(c => c.Items).Where(c => c.Status == Enums.OrderStatus.Recieved).OrderByDescending(c => c.Id).ToListAsync();

            return View(orders);
        }
    }
}
