using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Entities
{
    public class Comment : BaseEntity
    {
        public Comment()
        {
            
        }
        public Comment(int appUserId, int videoId, string content)
        {
            AppUserId = appUserId;
            VideoId = videoId;
            Content = content;
        }

        // FK = AppUserId and FK = VideoId
        public int AppUserId { get; set; }
        public int VideoId { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        // Navigations
        public AppUser AppUser { get; set; }
        public Video Video { get; set; }
    }
}
