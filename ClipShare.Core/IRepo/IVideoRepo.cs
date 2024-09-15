﻿using ClipShare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.IRepo
{
    public interface IVideoRepo : IBaseRepo<Video>
    {
        Task<int> GetUserIdByVideoId(int videoId);
    }
}
