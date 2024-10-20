using ClipShare.Core.IRepo;
using ClipShare.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClipShare.Controllers
{
    public class CoreController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private Context _context;
        private IConfiguration _configuration;
        protected IUnitOfWork UnitOfWork => _unitOfWork ??= HttpContext.RequestServices.GetService<IUnitOfWork>();
        protected Context Context => _context ??= HttpContext.RequestServices.GetService<Context>();
        protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>();
    }
}
