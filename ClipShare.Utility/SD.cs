using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;

namespace ClipShare.Utility
{
    public static class SD
    {
        public const string AdminRole = "admin";
        public const string ModeratorRole = "moderator";
        public const string UserRole = "user";
        public static readonly List<string> Roles = new List<string> { AdminRole, UserRole, ModeratorRole};
        public const int MB = 1000000;

        public static string IsActive(this IHtmlHelper html, string controller, string action, string cssClass = "active")
        {
            var routeData = html.ViewContext.RouteData;
            var routeAction = routeData.Values["action"]?.ToString();
            var routeController = routeData.Values["controller"]?.ToString();

            var retrunActive = controller == routeController && action == routeAction;

            return retrunActive ? cssClass : string.Empty;
        }

        public static string GetRandomeName()
        {
            var randomNumber = new byte[10];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
