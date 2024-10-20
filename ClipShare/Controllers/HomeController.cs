using ClipShare.Core.DTOs;
using ClipShare.Core.Pagination;
using ClipShare.Extensions;
using ClipShare.Utility;
using ClipShare.ViewModels;
using ClipShare.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ClipShare.Controllers
{
    public class HomeController : CoreController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(string page)
        {
            var toReturn = new Home_vm();

            if (User.Identity.IsAuthenticated)
            {
                toReturn.Page = page;

                if (page == null || page == "Home")
                {
                    var allCategories = await UnitOfWork.CategoryRepo.GetAllAsync();

                    var categoryList = allCategories.Select(category => new SelectListItem
                    {
                        Text = category.Name,
                        Value = category.Id.ToString()
                    }).ToList();

                    categoryList.Insert(0, new SelectListItem
                    {
                        Text = "All",
                        Value = "0",
                        Selected = true
                    });

                    toReturn.CategoryDropdown = categoryList;
                }
            }

            return View(toReturn);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region API Endpoints
        [Authorize(Roles = $"{SD.UserRole}")]
        [HttpGet]
        public async Task<IActionResult> GetVideosForHomeGrid(HomeParameters parameters)
        {
            var items = await UnitOfWork.VideoRepo.GetVideosForHomeGridAsync(parameters);
            var paginatedResults = new PaginatedResult<VideoForHomeGridDto>(items, items.TotalItemsCount, items.PageNumber, items.PageSize, items.TotalPages);

            return Json(new ApiResponse(200, result: paginatedResults));
        }

        [Authorize(Roles = $"{SD.UserRole}")]
        [HttpGet]
        public async Task<IActionResult> GetSubscriptions()
        {
            var userSubscribedChannels = await Context.Subscribe
                .Where(x => x.AppUserId == User.GetUserId())
                // project the result into an anonymous object
                .Select(x => new
                {
                    Id = x.ChannelId,
                    ChannelName = x.Channel.Name,
                    VideosCount = x.Channel.Videos.Count
                }).ToListAsync();

            return Json(new ApiResponse(200, result: userSubscribedChannels));
        }

        [Authorize(Roles = $"{SD.UserRole}")]
        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            var userWatchedVideoHistory = await Context.VideoView
                .Where(x => x.AppUserId == User.GetUserId())
                // project the result into an anonymous object
                .Select(x => new
                {
                    Id = x.VideoId,
                    x.Video.Title,
                    ChannelName = x.Video.Channel.Name,
                    ChannelId = x.Video.Channel.Id,
                    LastVisitTimeAgo = SD.TimeAgo(x.LastVisit),
                    x.LastVisit
                }).ToListAsync();

            return Json(new ApiResponse(200, result: userWatchedVideoHistory));
        }

        [Authorize(Roles = $"{SD.UserRole}")]
        [HttpGet]
        public async Task<IActionResult> GetLikesDislikesVideos(bool liked)
        {
            var userLikedDislikedVideos = await Context.LikeDislike
                .Where(x => x.AppUserId == User.GetUserId() && x.Liked == liked)
                // project the result into an anonymous object
                .Select(x => new
                {
                    Id = x.VideoId,
                    x.Video.Title,
                    x.Video.ThumbnailUrl,
                    ChannelName = x.Video.Channel.Name,
                    ChannelId = x.Video.Channel.Id,
                    CreatedAtTimeAgo = SD.TimeAgo(x.Video.CreatedAt),
                    x.Video.CreatedAt
                }).ToListAsync();

            return Json(new ApiResponse(200, result: userLikedDislikedVideos));
        }
        #endregion
    }
}
