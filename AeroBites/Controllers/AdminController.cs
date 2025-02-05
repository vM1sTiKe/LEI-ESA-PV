using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
