using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.DTOs
{
    public class VideoForHomeGridDto
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
        public string ChannelName { get; set; }
        public int ChannelId { get; set; }
        public int CategoryId { get; set; }
    }
}
