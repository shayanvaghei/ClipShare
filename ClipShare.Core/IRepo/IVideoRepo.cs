using ClipShare.Core.DTOs;
using ClipShare.Core.Entities;
using ClipShare.Core.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.IRepo
{
    public interface IVideoRepo : IBaseRepo<Video>
    {
        Task<int> GetUserIdByVideoIdAsync(int videoId);
        Task<PaginatedList<VideoGridChannelDto>> GetVideosForChannelGridAsync(int channelId, BaseParameters parameters);
        Task<PaginatedList<VideoForHomeGridDto>> GetVideosForHomeGridAsync(HomeParameters parameters);
        Task RemoveVideoAsync(int videoId);
    }
}
