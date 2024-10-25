using ClipShare.Utility;
using ClipShare.ViewModels.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClipShare.Controllers
{
    [Authorize(Roles = $"{SD.ModeratorRole}")]
    public class ModeratorController : CoreController
    {
        public async Task<IActionResult> AllVideos()
        {
            var videos = await UnitOfWork.VideoRepo.GetAllAsync(includeProperties: "Category,Channel");

            var toReturn = Mapper.Map<IEnumerable<VideoDisplayGrid_vm>>(videos);

            return View(toReturn);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await Context.Video
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.ThumbnailUrl,
                    x.Title
                }).FirstOrDefaultAsync();

            if (video != null)
            {
                PhotoService.DeletePhotoLocally(video.ThumbnailUrl);
                await UnitOfWork.VideoRepo.RemoveVideoAsync(video.Id);
                await UnitOfWork.CompleteAsync();

                TempData["notification"] = $"true;Deleted;Video of {video.Title} has been deleted";
                return RedirectToAction("AllVideos");
            }

            TempData["notification"] = $"false;Not Found;Requested video was not found";
            return RedirectToAction("AllVideos");
        }
    }
}
