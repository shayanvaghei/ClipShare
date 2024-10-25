using System;

namespace ClipShare.ViewModels.Moderator
{
    public class VideoDisplayGrid_vm
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
