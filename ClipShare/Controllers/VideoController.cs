using ClipShare.Core.DTOs;
using ClipShare.Core.Entities;
using ClipShare.Core.Pagination;
using ClipShare.Extensions;
using ClipShare.Services.IServices;
using ClipShare.Utility;
using ClipShare.ViewModels;
using ClipShare.ViewModels.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Watch(int id)
        {
            // inefficient way of fetching the videos with lots of include properties with uneccessary columns
            // var toReturn = await GetVideoWatch_vmWithIncludeProperties(id);

            // efficient way of fetching the video from the database and only takes the column that we are interested in the query
            var toReturn = await GetVideoWatch_vmWithProjections(id);

            if (toReturn != null)
            {
                var userIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                await UnitOfWork.VideoViewRepo.HandleVideoViewAsync(User.GetUserId(), id, userIpAddress);
                await UnitOfWork.CompleteAsync();

                return View(toReturn);
            }

            TempData["notification"] = "false;Not Found;Requested video was not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment_vm model)
        {
            var video = await UnitOfWork.VideoRepo.GetFirstOrDefaultAsync(x => x.Id == model.PostComment.VideoId, "Comments");
            if (video != null)
            {
                video.Comments.Add(new Comment(User.GetUserId(), model.PostComment.VideoId, model.PostComment.Content.Trim()));
                await UnitOfWork.CompleteAsync();

                return RedirectToAction("Watch", new { id = model.PostComment.VideoId });
            }

            TempData["notification"] = "false;Not Found;Requested video was not found";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> GetVideoFile(int videoId)
        {
            var fetcehdVideoFile = await UnitOfWork.VideoFileRepo.GetFirstOrDefaultAsync(x => x.VideoId == videoId);
            if (fetcehdVideoFile != null)
            {
                return File(fetcehdVideoFile.Contents, fetcehdVideoFile.ContentType);
            }

            TempData["notification"] = "false;Not Found;Requested video was not found";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DownloadVideoFile(int videoId)
        {
            var fetchedVideo = await UnitOfWork.VideoRepo.GetFirstOrDefaultAsync(x => x.Id == videoId, "VideoFile");
            if (fetchedVideo != null)
            {
                string fileDownloadName = fetchedVideo.Title + fetchedVideo.VideoFile.Extension;
                return File(fetchedVideo.VideoFile.Contents, fetchedVideo.VideoFile.ContentType, fileDownloadName);
            }

            TempData["notification"] = "false;Not Found;Requested video was not found";
            return RedirectToAction("Index", "Home");
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

                var userId = await UnitOfWork.VideoRepo.GetUserIdByVideoIdAsync(id);
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
                            VideoFile = new VideoFile
                            {
                                ContentType = model.VideoUpload.ContentType,
                                Contents = GetContentsAsync(model.VideoUpload).GetAwaiter().GetResult(),
                                Extension = SD.GetFileExtension(model.VideoUpload.ContentType)
                            },
                            CategoryId = model.CategoryId,
                            ChannelId = UnitOfWork.ChannelRepo.GetChannelIdByUserId(User.GetUserId()).GetAwaiter().GetResult(),
                            ThumbnailUrl = PhotoService.UploadPhotoLocally(model.ImageUpload)
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
                            fetchedVideo.ThumbnailUrl = PhotoService.UploadPhotoLocally(model.ImageUpload, fetchedVideo.ThumbnailUrl);
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

        #region API Endpoints
        [HttpGet]
        public async Task<IActionResult> GetVideosForChannelGrid(BaseParameters parameters)
        {
            var userChannelId = await UnitOfWork.ChannelRepo.GetChannelIdByUserId(User.GetUserId());
            var videosForGrid = await UnitOfWork.VideoRepo.GetVideosForChannelGridAsync(userChannelId, parameters);
            var paginatedResults = new PaginatedResult<VideoGridChannelDto>(videosForGrid, videosForGrid.TotalItemsCount,
                videosForGrid.PageNumber, videosForGrid.PageSize, videosForGrid.TotalPages);

            return Json(new ApiResponse(200, result: paginatedResults));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await Context.Video
                .Where(x => x.Id == id && x.Channel.AppUserId == User.GetUserId())
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

                return Json(new ApiResponse(200, "Deleted", "Your video of " + video.Title + " has been deleted"));
            }
            return Json(new ApiResponse(404, message: "The requested video was not found"));
        }


        [HttpPut]
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
                    await UnitOfWork.CompleteAsync();
                    return Json(new ApiResponse(200, "Subscribed", "Subscribed"));
                }
                else
                {
                    // Unsubscribe
                    channel.Subscribers.Remove(fetchedSubscribe);
                    await UnitOfWork.CompleteAsync();
                    return Json(new ApiResponse(200, "Unsubscribed", "Unsubscribed"));
                }
            }

            return Json(new ApiResponse(404, message: "Channel was not found"));
        }

        [HttpPut]
        public async Task<IActionResult> LikeDislikeVideo(int videoId, string action, bool like)
        {
            var video = await UnitOfWork.VideoRepo.GetFirstOrDefaultAsync(x => x.Id == videoId, "LikeDislikes");
            if (video != null)
            {
                int userId = User.GetUserId();

                var fetchedLikeDislike = video.LikeDislikes.Where(x => x.VideoId == videoId && x.AppUserId == userId).FirstOrDefault();
                string clienCommand = "";

                if (action.Equals("like"))
                {
                    if (fetchedLikeDislike == null)
                    {
                        // Adding LikeDislike with value of Like = true
                        video.LikeDislikes.Add(new LikeDislike(userId, videoId, true));
                        await UnitOfWork.CompleteAsync();
                        clienCommand = "addLike";
                        return Json(new ApiResponse(200, clienCommand));
                    }
                    else
                    {
                        // the user has whether liked or disliked previously and we need to update
                        if (fetchedLikeDislike.Liked == false)
                        {
                            // User was previously disliked the video and now decided to like the video so Liked becomes true
                            fetchedLikeDislike.Liked = true;
                            clienCommand = "removeDislike-addLike";
                        }
                        else
                        {
                            // User was previously liked the video, and now decided to not to like the video and still not Dislike the video
                            // so remove the LikeDislike from the database
                            video.LikeDislikes.Remove(fetchedLikeDislike);
                            clienCommand = "removeLike";
                        }

                        await UnitOfWork.CompleteAsync();
                        return Json(new ApiResponse(200, clienCommand));
                    }
                }
                else if (action.Equals("dislike"))
                {
                    if (fetchedLikeDislike == null)
                    {
                        // Adding LikeDislike with value of Like = false
                        video.LikeDislikes.Add(new LikeDislike(userId, videoId, false));
                        await UnitOfWork.CompleteAsync();
                        clienCommand = "addDislike";
                        return Json(new ApiResponse(200, clienCommand));
                    }
                    else
                    {
                        // the user has whether liked or disliked previously and we need to update
                        if (fetchedLikeDislike.Liked == true)
                        {
                            // User was previously liked the video and now decided to dislike the video so Liked becomes false
                            fetchedLikeDislike.Liked = false;
                            clienCommand = "removeLike-addDislike";
                        }
                        else
                        {
                            // User was previously disliked the video, and now decided to not to dislike the video and still not Like the video
                            // so remove the LikeDislike from the database
                            video.LikeDislikes.Remove(fetchedLikeDislike);
                            clienCommand = "removeDislike";
                        }

                        await UnitOfWork.CompleteAsync();
                        return Json(new ApiResponse(200, clienCommand));
                    }
                }
                else
                {
                    return Json(new ApiResponse(400, message: "Invalid action"));
                }
            }

            return Json(new ApiResponse(404, message: "Requested video was not found"));
        }
        #endregion

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

        private async Task<VideoWatch_vm> GetVideoWatch_vmWithIncludeProperties(int id)
        {
            // with having DOT (.) we can have theninclude eg: "Channel.Subscribers"
            var fetchedVideo = await UnitOfWork.VideoRepo.GetFirstOrDefaultAsync(x => x.Id == id, "Channel.Subscribers,LikeDislikes,Comments.AppUser,Viewers");
            if (fetchedVideo != null)
            {
                var toReturn = new VideoWatch_vm();
                int userId = User.GetUserId();

                toReturn.Id = fetchedVideo.Id;
                toReturn.Title = fetchedVideo.Title;
                toReturn.Description = fetchedVideo.Description;
                toReturn.CreatedAt = fetchedVideo.CreatedAt;
                toReturn.ChannelId = fetchedVideo.ChannelId;
                toReturn.ChannelName = fetchedVideo.Channel.Name;

                toReturn.IsSubscribed = fetchedVideo.Channel.Subscribers.Any(x => x.AppUserId == userId);
                toReturn.IsLiked = fetchedVideo.LikeDislikes.Any(x => x.AppUserId == userId && x.Liked == true);
                toReturn.IsDisiked = fetchedVideo.LikeDislikes.Any(x => x.AppUserId == userId && x.Liked == false);

                toReturn.SubscribersCount = fetchedVideo.Channel.Subscribers.Count();
                toReturn.ViewersCount = fetchedVideo.Viewers.Select(X => X.NumberOfVisit).Sum();
                toReturn.LikesCount = fetchedVideo.LikeDislikes.Where(x => x.Liked == true).Count();
                toReturn.DislikesCount = fetchedVideo.LikeDislikes.Where(x => x.Liked == false).Count();

                toReturn.CommentVM.PostComment.VideoId = id;
                toReturn.CommentVM.AvailableComments = fetchedVideo.Comments.Select(x => new AvailableComment_vm
                {
                    FromName = x.AppUser.Name,
                    FromChannelId = UnitOfWork.ChannelRepo.GetChannelIdByUserId(x.AppUserId).GetAwaiter().GetResult(),
                    PostedAt = x.PostedAt,
                    Content = x.Content,
                });

                return toReturn;
            }

            return null;
        }

        private async Task<VideoWatch_vm> GetVideoWatch_vmWithProjections(int id)
        {
            int userId = User.GetUserId();
            var toReturn = await Context.Video
                .Where(x => x.Id == id)
                .Select(x => new VideoWatch_vm
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    ChannelId = x.ChannelId,
                    ChannelName = x.Channel.Name,
                    IsSubscribed = x.Channel.Subscribers.Any(s => s.AppUserId == userId),
                    IsLiked = x.LikeDislikes.Any(l => l.AppUserId == userId && l.Liked == true),
                    IsDisiked = x.LikeDislikes.Any(l => l.AppUserId == userId && l.Liked == false),
                    SubscribersCount = x.Channel.Subscribers.Count(),
                    ViewersCount = x.Viewers.Select(v => v.NumberOfVisit).Sum(),
                    LikesCount = x.LikeDislikes.Where(l => l.Liked == true).Count(),
                    DislikesCount = x.LikeDislikes.Where(l => l.Liked == false).Count(),
                    CommentVM = new Comment_vm
                    {
                        PostComment = new CommentPost_vm
                        {
                            VideoId = x.Id
                        },
                        AvailableComments = x.Comments.Select(c => new AvailableComment_vm
                        {
                            FromName = c.AppUser.Name,
                            FromChannelId = c.AppUser.Channel.Id,
                            PostedAt = c.PostedAt,
                            Content = c.Content,
                        })
                    }
                })
                .FirstOrDefaultAsync();

            return toReturn;
        }
        #endregion
    }
}
