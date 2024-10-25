using ClipShare.Core.Entities;
using ClipShare.Extensions;
using ClipShare.Utility;
using ClipShare.ViewModels;
using ClipShare.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ClipShare.Controllers
{
    [Authorize(Roles = $"{SD.UserRole}")]
    public class MemberController : CoreController
    {

        public async Task<IActionResult> Channel(int id)
        {
            var fetchedChannel = await Context.Channel
                .Where(x => x.Id == id)
                .Select(x => new MemberChannel_vm
                {
                    ChannelId = x.Id,
                    Name = x.Name,
                    About = x.About,
                    CreatedAt = x.CreatedAt,
                    NumberOfAvailableVideos = x.Videos.Count(),
                    NumberOfSubscribers = x.Subscribers.Count(),
                    UserIsSubscribed = x.Subscribers.Any(s => s.AppUserId == User.GetUserId()),
                }).FirstOrDefaultAsync();

            if (fetchedChannel != null)
            {
                return View(fetchedChannel);
            }

            TempData["notification"] = "false;Not Found;Requested channel was not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeChannel(int channelId)
        {
            var channel = await UnitOfWork.ChannelRepo.GetFirstOrDefaultAsync(x => x.Id == channelId, "Subscribers");

            if (channel != null)
            {
                int userId = User.GetUserId();

                var fetchedSubscribe = channel.Subscribers.Where(x => x.ChannelId == channelId && x.AppUserId == userId).FirstOrDefault();

                if (fetchedSubscribe == null)
                {
                    // Subscribe
                    channel.Subscribers.Add(new Subscribe(userId, channelId));
                }
                else
                {
                    // Unsubscribe
                    channel.Subscribers.Remove(fetchedSubscribe);
                }

                await UnitOfWork.CompleteAsync();
                return RedirectToAction("Channel", new { id = channelId });
            }

            TempData["notification"] = "false;Not Found;Requested channel was not found";
            return RedirectToAction("Index", "Home");

        }

        #region API Endpoints

        [HttpGet]
        public async Task<IActionResult> GetMemberChannelVideos(int channelId)
        {
            var channelVideos = await Context.Video
             .Where(x => x.ChannelId == channelId)
             .Select(x => new
             {
                 x.Id,
                 x.Title,
                 x.ThumbnailUrl,
                 CreatedAtTimeAgo = SD.TimeAgo(x.CreatedAt),
                 x.CreatedAt,
                 NumberOfViews = x.Viewers.Count(),
             })
             .ToListAsync();

            return Json(new ApiResponse(200, result: channelVideos));

        }
        #endregion
    }
}
