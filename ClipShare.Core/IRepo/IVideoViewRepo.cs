using ClipShare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.IRepo
{
    public interface IVideoViewRepo : IBaseRepo<VideoView>
    {
        Task HandleVideoViewAsync(int userId, int videoId, string ipAddress);
    }
}
