using System.Security.Claims;

namespace DateApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier)??throw new Exception("cannot get id from token");
        }
    }
}
