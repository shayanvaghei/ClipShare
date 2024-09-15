using ClipShare.Core.Entities;
using ClipShare.Extensions;
using ClipShare.Services.IServices;
using ClipShare.Utility;
using ClipShare.ViewModels.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClipShare.Controllers
{
    [Authorize(Roles = $"{SD.UserRole}")]
    public class VideoController : CoreController
    {
        private readonly IPhotoService _photoService;

        public VideoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }
        public async Task<IActionResult> CreateEditVideo(int id)
        {
            if (!await UnitOfWork.ChannelRepo.AnyAsync(x => x.AppUserId == User.GetUserId()))
            {
                TempData["notfication"] = "false;Not Found;No channel associated with your account was found.";
                return RedirectToAction("Index", "Channel");
            }

            var toReturn = new VideoAddEdit_vm();
            toReturn.ImageContentTypes = string.Join(",", AcceptableContentTypes("image"));
            toReturn.VideoContentTypes = string.Join(",", AcceptableContentTypes("video"));

            if (id > 0)
            {
                // edit part

                var userId = await UnitOfWork.VideoRepo.GetUserIdByVideoId(id);
                if (!userId.Equals(User.GetUserId()))
                {
                    TempData["notfication"] = "false;Not Found;Requested video was not found.";
                    return RedirectToAction("Index", "Channel");
                }

                var fetchedVideo = await UnitOfWork.VideoRepo.GetByIdAsync(id);
                if (fetchedVideo == null)
                {
                    TempData["notfication"] = "false;Not Found;Requested video was not found.";
                    return RedirectToAction("Index", "Channel");
                }

                toReturn.Id = fetchedVideo.Id;
                toReturn.Title = fetchedVideo.Title;
                toReturn.Description = fetchedVideo.Description;
                toReturn.CategoryId = fetchedVideo.CategoryId;
                toReturn.ImageUrl = fetchedVideo.ThumbnailUrl;
            }

            toReturn.CategoryDropdown = await GetCategoryDropdownAsync();

            return View(toReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditVideo(VideoAddEdit_vm model)
        {
            if (ModelState.IsValid)
            {
                bool proceed = true;

                if (model.Id == 0)
                {
                    // adding some security check for create
                    if (model.ImageUpload == null)
                    {
                        ModelState.AddModelError("ImageUpload", "Please upload thumbnail");
                        proceed = false;
                    }

                    if (proceed && model.VideoUpload == null)
                    {
                        ModelState.AddModelError("VideoUpload", "Please upload your video");
                        proceed = false;
                    }
                }

                if (model.ImageUpload != null)
                {
                    if (proceed && !IsAcceptableContentType("image", model.ImageUpload.ContentType))
                    {
                        ModelState.AddModelError("ImageUpload", string.Format("Invalid content type. It must be one of the following: {0}",
                            string.Join(", ", AcceptableContentTypes("image"))));
                        proceed = false;
                    }

                    if (proceed && model.ImageUpload.Length > int.Parse(Configuration["FileUpload:ImageMaxSizeInMB"]) * SD.MB)
                    {
                        ModelState.AddModelError("ImageUpload", string.Format("The uploaded file should not exceed {0} MB",
                            int.Parse(Configuration["FileUpload:ImageMaxSizeInMB"])));
                        proceed = false;
                    }
                }

                if (model.VideoUpload != null)
                {
                    if (proceed && !IsAcceptableContentType("video", model.VideoUpload.ContentType))
                    {
                        ModelState.AddModelError("VideoUpload", string.Format("Invalid content type. It must be one of the following: {0}",
                            string.Join(", ", AcceptableContentTypes("video"))));
                        proceed = false;
                    }

                    if (proceed && model.VideoUpload.Length > int.Parse(Configuration["FileUpload:VideoMaxSizeInMB"]) * SD.MB)
                    {
                        ModelState.AddModelError("VideoUpload", string.Format("The uploaded file should not exceed {0} MB",
                            int.Parse(Configuration["FileUpload:VideoMaxSizeInMB"])));
                        proceed = false;
                    }
                }

                if (proceed)
                {
                    string title = "";
                    string message = "";

                    if (model.Id == 0)
                    {
                        // for create
                        var videoToAdd = new Video()
                        {
                            Title = model.Title,
                            Description = model.Description,
                            ContentType = model.VideoUpload.ContentType,
                            Contents = GetContentsAsync(model.VideoUpload).GetAwaiter().GetResult(),
                            CategoryId = model.CategoryId,
                            ChannelId = UnitOfWork.ChannelRepo.GetChannelIdByUserId(User.GetUserId()).GetAwaiter().GetResult(),
                            ThumbnailUrl = _photoService.UploadPhotoLocally(model.ImageUpload)
                        };

                        UnitOfWork.VideoRepo.Add(videoToAdd);

                        title = "Created";
                        message = "New video has been created";
                    }
                    else
                    {
                        // for update
                        var fetchedVideo = await UnitOfWork.VideoRepo.GetByIdAsync(model.Id);
                        if (fetchedVideo == null)
                        {
                            TempData["notification"] = "false;Not Found;Requested video was not found";
                            return RedirectToAction("Index", "Channel");
                        }

                        fetchedVideo.Title = model.Title;
                        fetchedVideo.Description = model.Description;
                        fetchedVideo.CategoryId = model.CategoryId;

                        if (model.ImageUpload != null)
                        {
                            fetchedVideo.ThumbnailUrl = _photoService.UploadPhotoLocally(model.ImageUpload, fetchedVideo.ThumbnailUrl);
                        }

                        title = "Edited";
                        message = "Video has been updated";
                    }

                    TempData["notification"] = $"true;{title};{message}";
                    await UnitOfWork.CompleteAsync();

                    return RedirectToAction("Index", "Channel");
                }
            }

            model.CategoryDropdown = await GetCategoryDropdownAsync();
            return View(model);
        }

        #region Private Methods
        public async Task<IEnumerable<SelectListItem>> GetCategoryDropdownAsync()
        {
            var allCategories = await UnitOfWork.CategoryRepo.GetAllAsync();

            return allCategories.Select(category => new SelectListItem()
            {
                Text = category.Name,
                Value = category.Id.ToString()
            });
        }

        private string[] AcceptableContentTypes(string type)
        {
            if (type.Equals("image"))
            {
                return Configuration.GetSection("FileUpload:ImageContentTypes").Get<string[]>();
            }
            else
            {
                return Configuration.GetSection("FileUpload:VideoContentTypes").Get<string[]>();
            }
        }

        private bool IsAcceptableContentType(string type, string contentType)
        {
            var allowedContentTypes = AcceptableContentTypes(type);
            foreach (var allowedContentType in allowedContentTypes)
            {
                if (contentType.ToLower().Equals(allowedContentType.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<byte[]> GetContentsAsync(IFormFile file)
        {
            byte[] contents;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            contents = memoryStream.ToArray();
            return contents;
        }
        #endregion
    }
}
