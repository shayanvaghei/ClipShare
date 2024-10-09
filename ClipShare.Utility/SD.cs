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

        public static string IsActivePage(this IHtmlHelper html, string page)
        {
            var currentPage = html.ViewContext.HttpContext.Request.Query["page"].ToString();
            var isPageMatch = currentPage == page;

            // default Home page
            if (string.IsNullOrEmpty(currentPage) && page == "Home") return "active";

            return isPageMatch ? "active" : string.Empty;
        }

        public static int GetRandomNumber(int minValue, int maxValue, int seed)
        {
            Random random = new Random(seed);
            return random.Next(minValue, maxValue);
        }

        public static DateTime GetRandomDate(DateTime minDate, DateTime maxDate, int seed)
        {
            Random random = new Random(seed);
            int range = (maxDate - minDate).Days;
            return minDate.AddDays(random.Next(range + 1));
        }
    }
}
