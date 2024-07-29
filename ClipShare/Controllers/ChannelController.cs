using ClipShare.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClipShare.Controllers
{
    [Authorize(Roles = $"{SD.UserRole}")]
    public class ChannelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
