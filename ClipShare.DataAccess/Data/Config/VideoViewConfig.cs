using ClipShare.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Data.Config
{
    public class VideoViewConfig : IEntityTypeConfiguration<VideoView>
    {
        public void Configure(EntityTypeBuilder<VideoView> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.HasOne(a => a.AppUser).WithMany(c => c.Histories).HasForeignKey(c => c.AppUserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.Video).WithMany(c => c.Viewers).HasForeignKey(c => c.VideoId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
