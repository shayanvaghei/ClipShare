using System;

namespace ClipShare.ViewModels.Video
{
    public class VideoWatch_vm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisiked { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public int ViewersCount { get; set; }
        public int SubscribersCount { get; set; }
        public Comment_vm CommentVM { get; set; } = new();
    }
}
