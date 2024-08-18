using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.IRepo
{
    public interface IUnitOfWork : IDisposable
    {
        IChannelRepo ChannelRepo { get; }
        ICategoryRepo CategoryRepo { get; }
        IVideoRepo VideoRepo { get; }
        Task<bool> CompleteAsync();
    }
}
