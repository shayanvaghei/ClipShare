using Microsoft.AspNetCore.Http;

namespace ClipShare.Services.IServices
{
    public interface IPhotoService
    {
        string UploadPhotoLocally(IFormFile photo, string oldPhotoUrl = "");
        void DeletePhotoLocally(string photoUrl);
    }
}
