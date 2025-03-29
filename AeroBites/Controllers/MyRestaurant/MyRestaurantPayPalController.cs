using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers.MyRestaurant
{
    [Route("/MyRestaurant/PayPal")]
    public class MyRestaurantPayPalController : Controller
    {
        [HttpGet("Set")]
        public string Set()
        {
            // Example of endpoint, set code here
            return "dwewqewq";
        }
    }
}
