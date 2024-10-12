using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Entities
{
    public class Subscribe
    {
        public Subscribe()
        {
            
        }
        public Subscribe(int appUserId, int channelId)
        {
            AppUserId = appUserId;
            ChannelId = channelId;
        }

        // PK (AppUserId, ChannelId)
        // FK = AppUserId and FK = ChannelId
        public int AppUserId { get; set; }
        public int ChannelId { get; set; }

        // Navigations
        public AppUser AppUser { get; set; }
        public Channel Channel { get; set; }
    }
}
