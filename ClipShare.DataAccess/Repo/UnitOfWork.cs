using ClipShare.Core.IRepo;
using ClipShare.DataAccess.Data;
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

        public UnitOfWork(Context context)
        {
            _context = context;
        }

        public IChannelRepo ChannelRepo => new ChannelRepo(_context);
        public ICategoryRepo CategoryRepo => new CategoryRepo(_context);
        public IVideoRepo VideoRepo => new VideoRepo(_context);

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
