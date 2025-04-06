using System.Security.Claims;
using AeroBites.Data;

namespace AeroBites
{
    public static class Utils
    {
        /// <summary>
        /// This method returns the value stored in the 'NameIdentifier' Claim
        /// </summary>
        public static int GetId(this ClaimsPrincipal user) {
            return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

        /// <summary>
        /// This method returns saying if the user is or not admin
        /// </summary>
        public static bool IsAdmin(this ClaimsPrincipal user) {
            return (user.FindFirstValue("IsAdmin") ?? "").Equals("True");
        }
    }

    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AeroBitesContext>();

            context.Database.EnsureCreated();
        }
    }
}