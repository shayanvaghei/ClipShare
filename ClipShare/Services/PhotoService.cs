using ClipShare.Services.IServices;
using ClipShare.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace ClipShare.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public PhotoService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public string UploadPhotoLocally(IFormFile photo, string oldPhotoUrl = "")
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            string uploadsDriectory = Path.Combine(webRootPath, @"images\thumbnails");

            // check if the directory exists, othewise create
            if (!Directory.Exists(uploadsDriectory))
            {
                Directory.CreateDirectory(uploadsDriectory);
            }

            string fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(photo.FileName);

            if (!string.IsNullOrEmpty(oldPhotoUrl))
            {
                // replace the image
                var oldImagePath = Path.Combine(webRootPath, oldPhotoUrl.TrimStart('\\'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            using var fileStream = new FileStream(Path.Combine(uploadsDriectory, fileName + extension), FileMode.Create);
            photo.CopyTo(fileStream);

            return @"\images\thumbnails\" + fileName + extension;
        }

        public void DeletePhotoLocally(string photoUrl)
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            string uploadsDriectory = Path.Combine(webRootPath, @"images\thumbnails");

            // Delete the image
            var oldImagePath = Path.Combine(webRootPath, photoUrl.TrimStart('\\'));
            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }
        }
    }
}
