using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
