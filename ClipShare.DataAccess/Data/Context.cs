using ClipShare.Core.Entities;
using ClipShare.DataAccess.Data.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Data
{
    public class Context : IdentityDbContext<AppUser, AppRole, int>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Channel> Channel { get; set; }
        public DbSet<Video> Video { get; set; }
        public DbSet<VideoView> VideoView { get; set; }
        public DbSet<Subscribe> Subscribe { get; set; }
        public DbSet<LikeDislike> LikeDislike { get; set; }
        public DbSet<Comment> Comment { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // shorter way of applying the manual configuration
            //$ builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // longer way of applying the manual configuration
            builder.ApplyConfiguration(new CommentConfig());
            builder.ApplyConfiguration(new SubscribeConfig());
            builder.ApplyConfiguration(new LikeDislikeConfig());
            builder.ApplyConfiguration(new VideoViewConfig());
        }
    }
}
