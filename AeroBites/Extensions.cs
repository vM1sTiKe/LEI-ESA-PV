using System.Security.Claims;

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
    }
}