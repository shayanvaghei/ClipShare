using ClipShare.Core.IRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClipShare.Controllers
{
    public class CoreController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        protected IUnitOfWork UnitOfWork => _unitOfWork ??= HttpContext.RequestServices.GetService<IUnitOfWork>();
        protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>();
    }
}
