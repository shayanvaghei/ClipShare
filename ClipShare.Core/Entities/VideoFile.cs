using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Entities
{
    public class VideoFile : BaseEntity
    {
        [Required]
        public string ContentType { get; set; }
        [Required]
        public byte[] Contents { get; set; }
        [Required]
        public string Extension { get; set; }

        public int VideoId { get; set; }
        // Navigation
        [ForeignKey("VideoId")]
        public Video Video { get; set; }
    }
}
