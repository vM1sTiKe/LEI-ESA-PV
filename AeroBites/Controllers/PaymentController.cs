using AeroBites.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AeroBitesContext _context;

        public PaymentController(AeroBitesContext context)
        {
            _context = context;
        }

        public IActionResult Checkout()
        {
            var userId = User.GetId();

            var cart = _context.Cart
                .FirstOrDefault(c => c.AccountId == userId && c.Status == Enums.OrderStatus.Choosing);

            if(cart == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            cart.Items = _context.CartItem.Where(i => i.CartId == cart.Id).ToList();

            ViewBag.RestaurantId = cart.RestaurantId;

            return View(cart);
        }
    }
}
