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

        public async Task<int> GetUserIdByVideoId(int videoId)
        {
            return await _context.Video
                .Where(x => x.Id == videoId)
                .Select(x => x.Channel.AppUserId)
                .FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<VideoGridChannelDto>> GetVideosForChannelGrid(int channelId, BaseParameters parameters)
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
                    Views = SD.GetRandomNumber(1000, 500000, x.Id),
                    Comments = SD.GetRandomNumber(1, 100, x.Id),
                    Likes = SD.GetRandomNumber(10, 100, x.Id),
                    Dislikes = SD.GetRandomNumber(5, 100, x.Id),
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
                //"views-a" => query.OrderBy(u => u.Views.ToString()),
                //"views-d" => query.OrderByDescending(u => u.Views.ToString()),
                //"comments-a" => query.OrderBy(u => u.Comments),
                //"comments-d" => query.OrderByDescending(u => u.Comments),
                //"likes-a" => query.OrderBy(u => u.Likes),
                //"likes-d" => query.OrderByDescending(u => u.Likes),
                //"dislikes-a" => query.OrderBy(u => u.Dislikes),
                //"dislikes-d" => query.OrderByDescending(u => u.Dislikes),
                "category-a" => query.OrderBy(u => u.CategoryName),
                "category-d" => query.OrderByDescending(u => u.CategoryName),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            return await PaginatedList<VideoGridChannelDto>.CreateAsync(query.AsNoTracking(), parameters.PageNumber, parameters.PageSize);
        }
    }
}
