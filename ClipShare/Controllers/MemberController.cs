using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClipShare.Controllers
{
    [Authorize]
    public class MemberController : CoreController
    {

        public IActionResult Channel(int id)
        {
            return View();
        }
    }
}
