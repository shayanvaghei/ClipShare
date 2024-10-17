using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Entities
{
    public class VideoView : BaseEntity
    {
        // FK = AppUserId and FK = VideoId
        public int AppUserId { get; set; }
        public int VideoId { get; set; }


        // IP2Location
        public string IpAddress { get; set; }
        public int NumberOfVisit { get; set; } = 1;
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool Is_Proxy { get; set; }
        public DateTime LastVisit { get; set; } = DateTime.UtcNow;


        // Navigations
        public AppUser AppUser { get; set; }
        public Video Video { get; set; }
    }
}
