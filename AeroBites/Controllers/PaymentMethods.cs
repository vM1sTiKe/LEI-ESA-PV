using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class PaymentMethods : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Add()
        {
            // Código para adicionar novo metodo
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            // Codigo para remover método
            return RedirectToAction(nameof(Index));
        }
    }
}
