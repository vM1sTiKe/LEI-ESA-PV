using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AeroBites.Controllers
{
    public class AccountController : Controller
    {
        private readonly AeroBitesContext _context;

        public AccountController(AeroBitesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index), "Restaurant");
            }

            ViewBag.ClientId = "724687745332-an8kc4k4tpmv15tabt4okv163e4s56mm.apps.googleusercontent.com";
            ViewBag.LoginUri = "http://localhost:7263/account/signin";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn()
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

            if(!AccountExists(googleID))
            {
                CreateAccount(googleID);
            }

            var accountInfo = GetAccountByGoogleID(googleID);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, accountInfo.Id.ToString()),
                new Claim("GoogleId", accountInfo.GoogleId),
                new Claim("IsAdmin", accountInfo.IsAdmin.ToString())
            };

            var claimsIndentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(
                "Cookies", 
                new ClaimsPrincipal(claimsIndentity), 
                new AuthenticationProperties { IsPersistent = true }
            );

            return RedirectToAction(nameof(Index), "Restaurant");
        }

        public async Task<IActionResult> SignOff()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(string googleID)
        {
            return _context.Account.Any(account => account.GoogleId == googleID);
        }

        private void CreateAccount(string googleID, bool isAdmin = false)
        {
            if(!AccountExists(googleID))
            {
                var account = new Account
                {
                    GoogleId = googleID,
                    IsAdmin = false
                };

                _context.Account.Add(account);
                _context.SaveChanges();
            }
        }

        private Account? GetAccountByGoogleID(string googleID)
        {
            return _context.Account.FirstOrDefault(account => account.GoogleId == googleID);
        }

        private static JsonElement? DecodeJwt(string token)
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

        private static string PadBase64(string input)
        {
            var count = 3 - ((input.Length + 3) % 4);
            if (count == 0) return input;
            return input + new string('=', count);
        }
    }
}