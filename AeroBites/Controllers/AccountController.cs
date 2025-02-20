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

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="context">The context to interact with the database. This is injected by the dependency injection container.</param>
        public AccountController(AeroBitesContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the sign-in page.
        /// </summary>
        /// <returns>
        /// A ViewResult that renders the "Index" view.
        /// </returns>
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

        /// <summary>
        /// Handles the Google Sign-In process.
        /// </summary>
        /// <returns>
        /// A redirect to the "Index" action of the "Restaurant" controller if authentication succeeds.<br/>
        /// Returns a <see cref="BadRequestResult"/> if the Google ID token is missing or invalid.
        /// </returns>
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

        /// <summary>
        /// Handles the user sign-out by clearing authentication cookies.
        /// </summary>
        /// <returns>
        /// A redirect to the "Index" action after sucessful sign-out.
        /// </returns>
        public async Task<IActionResult> SignOff()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Check if the account exists for a given Google ID.
        /// </summary>
        /// <param name="googleID">The Google ID associated with the user.</param>
        /// <returns>
        /// <c>true</c> if the account exists; otherwise, <c>false</c>.
        /// </returns>
        private bool AccountExists(string googleID)
        {
            return _context.Account.Any(account => account.GoogleId == googleID);
        }

        /// <summary>
        /// Creates a new account for a given Google ID if it does not already exist.
        /// </summary>
        /// <param name="googleID">The Google ID of the new user.</param>
        /// <param name="isAdmin">Indicates wether the new user is an admin.<br/>
        /// Default is <c>false</c>.
        /// </param>
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

        /// <summary>
        /// Retrieves an account based on the Google ID.
        /// </summary>
        /// <param name="googleID">The Google ID of the user.</param>
        /// <returns>
        /// The account associated with the Google ID if found; otherwise, <c>null</c>.
        /// </returns>
        private Account? GetAccountByGoogleID(string googleID)
        {
            return _context.Account.FirstOrDefault(account => account.GoogleId == googleID);
        }

        /// <summary>
        /// Decodes a JWT token and extracts the payload as a JSON object.
        /// </summary>
        /// <param name="token">The JWT token to decode.</param>
        /// <returns>
        /// A <see cref="JsonElement"/> containing the decoded payload, or <c>null</c> if decoding fails.
        /// </returns>
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

        /// <summary>
        /// Ensures that a Base64 string has the correct padding required for decoding.
        /// </summary>
        /// <param name="input">The Base64 string to pad.</param>
        /// <returns>
        /// A correctly padded Base64 string.
        /// </returns>
        private static string PadBase64(string input)
        {
            var count = 3 - ((input.Length + 3) % 4);
            if (count == 0) return input;
            return input + new string('=', count);
        }
    }
}