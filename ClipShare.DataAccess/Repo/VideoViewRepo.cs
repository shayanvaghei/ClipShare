using ClipShare.Core.DTOs;
using ClipShare.Core.Entities;
using ClipShare.Core.IRepo;
using ClipShare.DataAccess.Data;
using ClipShare.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Repo
{
    public class VideoViewRepo : BaseRepo<VideoView>, IVideoViewRepo
    {
        private readonly Context _context;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public VideoViewRepo(Context context, IConfiguration config) : base(context)
        {
            _context = context;
            _config = config;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.ip2location.io")
            };
        }

        public async Task HandleVideoViewAsync(int userId, int videoId, string ipAddress)
        {
            var fetchedVideoView = await _context.VideoView
                .Where(x => x.AppUserId == userId && x.VideoId == videoId)
                .OrderByDescending(x => x.LastVisit)
                .FirstOrDefaultAsync();

            if (fetchedVideoView == null)
            {
                await AddVideoViewAsync(userId, videoId, ipAddress);
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                DateTime oneHourAfterLastVisit = fetchedVideoView.LastVisit.AddHours(1);

                if (now > oneHourAfterLastVisit && now.Date == fetchedVideoView.LastVisit.Date)
                {
                    // Last visit was more than one hour ago and still in the same day (today)
                    fetchedVideoView.LastVisit = DateTime.UtcNow;
                    fetchedVideoView.NumberOfVisit++;
                }

                if (fetchedVideoView.LastVisit.Date < now.Date)
                {
                    // Last visit was yesterday or more than one day ago
                    await AddVideoViewAsync(userId, videoId, ipAddress);
                }
            }
        }

        #region Private Methods
        private async Task AddVideoViewAsync(int userId, int videoId, string ipAddress)
        {
            var ip2LocationResult = await GetIP2LocationResultAsync(ipAddress);

            var videoViewToAdd = new VideoView
            {
                AppUserId = userId,
                VideoId = videoId,
                IpAddress = ipAddress,
                Country = ip2LocationResult.Country_Name,
                City = ip2LocationResult.City_Name,
                PostalCode = ip2LocationResult.Zip_Code,
                Is_Proxy = ip2LocationResult.Is_Proxy
            };

            Add(videoViewToAdd);
        }

        private async Task<IP2LocationResultDto> GetIP2LocationResultAsync(string ipAddress)
        {
            try
            {
                if (SD.LocalIpAddresses.Contains(ipAddress))
                {
                    return new IP2LocationResultDto();
                }
                else
                {
                    var result = await _httpClient.GetFromJsonAsync<IP2LocationResultDto>($"?Key={_config["IP2LocationAPIKey"]}&ip={ipAddress}&format=json");
                    return result;
                }
            }
            catch (Exception)
            {
                return new IP2LocationResultDto();
            }
        }
        #endregion
    }
}
