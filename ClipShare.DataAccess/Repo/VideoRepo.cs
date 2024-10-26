using ClipShare.Core.DTOs;
using ClipShare.Core.Entities;
using ClipShare.Core.IRepo;
using ClipShare.Core.Pagination;
using ClipShare.DataAccess.Data;
using ClipShare.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Repo
{
    public class VideoRepo : BaseRepo<Video>, IVideoRepo
    {
        private readonly Context _context;

        public VideoRepo(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetUserIdByVideoIdAsync(int videoId)
        {
            return await _context.Video
                .Where(x => x.Id == videoId)
                .Select(x => x.Channel.AppUserId)
                .FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<VideoGridChannelDto>> GetVideosForChannelGridAsync(int channelId, BaseParameters parameters)
        {
            var query = _context.Video
                .Include(x => x.Category)
                .Where(x => x.ChannelId == channelId)
                .Select(x => new VideoGridChannelDto
                {
                    Id = x.Id,
                    ThumbnailUrl = x.ThumbnailUrl,
                    Title = x.Title,
                    CreatedAt = x.CreatedAt,
                    CategoryName = x.Category.Name,
                    Views = x.Viewers.Count(),
                    Comments = x.Comments.Count(),
                    Likes = x.LikeDislikes.Where(l => l.Liked == true).Count(),
                    Dislikes = x.LikeDislikes.Where(l => l.Liked == false).Count(),
                })
                .AsQueryable();

            // a => ascending
            // d => descending
            query = parameters.SortBy switch
            {
                "title-a" => query.OrderBy(x => x.Title),
                "title-d" => query.OrderByDescending(x => x.Title),
                "date-a" => query.OrderBy(u => u.CreatedAt),
                "date-d" => query.OrderByDescending(u => u.CreatedAt),
                "views-a" => query.OrderBy(u => u.Views),
                "views-d" => query.OrderByDescending(u => u.Views),
                "comments-a" => query.OrderBy(u => u.Comments),
                "comments-d" => query.OrderByDescending(u => u.Comments),
                "likes-a" => query.OrderBy(u => u.Likes),
                "likes-d" => query.OrderByDescending(u => u.Likes),
                "dislikes-a" => query.OrderBy(u => u.Dislikes),
                "dislikes-d" => query.OrderByDescending(u => u.Dislikes),
                "category-a" => query.OrderBy(u => u.CategoryName),
                "category-d" => query.OrderByDescending(u => u.CategoryName),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            return await PaginatedList<VideoGridChannelDto>.CreateAsync(query.AsNoTracking(), parameters.PageNumber, parameters.PageSize);
        }

        public async Task<PaginatedList<VideoForHomeGridDto>> GetVideosForHomeGridAsync(HomeParameters parameters)
        {
            var query = _context.Video
                .Select(x => new VideoForHomeGridDto
                {
                    Id = x.Id,
                    ThumbnailUrl = x.ThumbnailUrl,
                    Title = x.Title,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    ChannelName = x.Channel.Name,
                    ChannelId = x.Channel.Id,
                    CategoryId = x.Category.Id,
                    Views = x.Viewers.Count()
                })
                .AsQueryable();

            if (parameters.CategoryId > 0)
            {
                query = query.Where(x => x.CategoryId == parameters.CategoryId);
            }

            if (!string.IsNullOrEmpty(parameters.SearchBy))
            {
                query = query.Where(x => x.Title.ToLower().Contains(parameters.SearchBy) || x.Description.ToLower().Contains(parameters.SearchBy));
            }

            return await PaginatedList<VideoForHomeGridDto>.CreateAsync(query.AsNoTracking(), parameters.PageNumber, parameters.PageSize);
        }

        public async Task RemoveVideoAsync(int videoId)
        {
            var video = await GetFirstOrDefaultAsync(x => x.Id == videoId, "Comments,LikeDislikes,Viewers");

            if (video != null)
            {
                if (video.Viewers != null)
                {
                    _context.VideoView.RemoveRange(video.Viewers);
                }

                if (video.Comments != null)
                {
                    _context.Comment.RemoveRange(video.Comments);
                }

                if (video.LikeDislikes != null)
                {
                    _context.RemoveRange(video.LikeDislikes);
                }

                Remove(video);
            }
        }
    }
}
