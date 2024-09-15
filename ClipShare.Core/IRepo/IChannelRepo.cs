using ClipShare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.IRepo
{
    public interface IChannelRepo : IBaseRepo<Channel>
    {
        Task<int> GetChannelIdByUserId(int userId);
    }
}
