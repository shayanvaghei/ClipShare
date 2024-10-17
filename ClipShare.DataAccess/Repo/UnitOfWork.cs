using ClipShare.Core.IRepo;
using ClipShare.DataAccess.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        private readonly IConfiguration _config;

        public UnitOfWork(Context context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IChannelRepo ChannelRepo => new ChannelRepo(_context);
        public ICategoryRepo CategoryRepo => new CategoryRepo(_context);
        public IVideoRepo VideoRepo => new VideoRepo(_context);
        public IVideoFileRepo VideoFileRepo => new VideoFileRepo(_context);
        public ICommentRepo CommentRepo => new CommentRepo(_context);
        public IVideoViewRepo VideoViewRepo => new VideoViewRepo(_context, _config);

        public async Task<bool> CompleteAsync()
        {
            bool result = false;

            if (_context.ChangeTracker.HasChanges())
            {
                result = await _context.SaveChangesAsync() > 0;
            }

            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
