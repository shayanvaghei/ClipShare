﻿using ClipShare.Core.Entities;
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
            // defining the primary key which is a combination of both AppUserId and VideoId
            builder.HasKey(x => new { x.AppUserId, x.VideoId });
            builder.HasOne(a => a.AppUser).WithMany(c => c.Histories).HasForeignKey(c => c.AppUserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.Video).WithMany(c => c.Viewers).HasForeignKey(c => c.VideoId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
