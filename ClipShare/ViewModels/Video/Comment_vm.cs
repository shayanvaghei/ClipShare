using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClipShare.ViewModels.Video
{
    public class Comment_vm
    {
        public CommentPost_vm PostComment { get; set; } = new();
        public IEnumerable<AvailableComment_vm> AvailableComments { get; set; }
    }

    public class CommentPost_vm
    {
        [Required]
        public int VideoId { get; set; }
        [Required]
        public string Content { get; set; }
    }

    public class AvailableComment_vm
    {
        public string Content { get; set; }
        public string FromName { get; set; }
        public int FromChannelId { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
