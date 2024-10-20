using System.Security.Claims;

namespace ClipShare.Extensions
{
    public static class UserClaimsExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string GetName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.GivenName)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public static int GetUserChannelId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.Sid)?.Value);
        }
    }
}
