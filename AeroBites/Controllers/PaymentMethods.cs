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

        [HttpPost]
        public IActionResult Default(int id)
        {
            // código para adicionar o método como default removendo o atual default.
            // se o metodo enviado ja for o default ele vai parar de ser default
            return RedirectToAction(nameof(Index));
        }
    }
}
