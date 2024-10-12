using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Entities
{
    public class LikeDislike
    {
        public LikeDislike()
        {
            
        }
        public LikeDislike(int appUserId, int videoId, bool liked)
        {
            AppUserId = appUserId;
            VideoId = videoId;
            Liked = liked;
        }

        // PK (AppUserId, VideoId)
        // FK = AppUserId and FK = VideoId
        public int AppUserId { get; set; }
        public int VideoId { get; set; }

        public bool Liked { get; set; }

        // Navigations
        public AppUser AppUser { get; set; }
        public Video Video { get; set; }
    }
}
