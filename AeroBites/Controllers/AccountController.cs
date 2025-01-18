using AeroBites.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace AeroBites.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.ClientId = _configuration["Authentication:Google:ClientId"];
            ViewBag.LoginUri = _configuration["Authentication:Google:LoginUri"];

            return View();
        }

        [HttpPost]
        public IActionResult SignIn()
        {
            var credential = HttpContext.Request.Form["credential"].FirstOrDefault();

            if(string.IsNullOrEmpty(credential))
            {
                return BadRequest("Google ID Token not found.");
            }

            var payload = DecodeJwt(credential);

            if(payload == null)
            {
                return BadRequest("Failed to decode JWT.");
            }

            string? googleID = payload.GetValueOrDefault().GetProperty("sub").GetString();

            var account = new Account
            {
                GoogleId = googleID,
                IsAdmin = false
            };

            var jsonString = JsonSerializer.Serialize(account);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(jsonString);

            HttpContext.Session.Set("AccountInfo", byteArray);

            Console.WriteLine(HttpContext.Session.GetString("AccountInfo"));

            return RedirectToAction("Index", "Restaurantes");
        }

        private JsonElement? DecodeJwt(string token)
        {
            try
            {
                var parts = token.Split('.');
                if(parts.Length != 3)
                {
                    return null;
                }

                var payload = parts[1];
                var jsonBytes = Convert.FromBase64String(PadBase64(payload));
                var jsonString = Encoding.UTF8.GetString(jsonBytes);
                return JsonSerializer.Deserialize<JsonElement>(jsonString);
            }
            catch
            {
                return null;
            }
        }

        private string PadBase64(string input)
        {
            var count = 3 - ((input.Length + 3) % 4);
            if (count == 0) return input;
            return input + new string('=', count);
        }
    }
}