using AutoMapper;
using ClipShare.Core.IRepo;
using ClipShare.DataAccess.Data;
using ClipShare.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClipShare.Controllers
{
    public class CoreController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IPhotoService _photoService;
        private Context _context;
        private IConfiguration _configuration;
        private IMapper _mapper;
        protected IUnitOfWork UnitOfWork => _unitOfWork ??= HttpContext.RequestServices.GetService<IUnitOfWork>();
        protected IPhotoService PhotoService => _photoService ??= HttpContext.RequestServices.GetService<IPhotoService>();
        protected Context Context => _context ??= HttpContext.RequestServices.GetService<Context>();
        protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
    }
}
