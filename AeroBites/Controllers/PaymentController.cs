using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
