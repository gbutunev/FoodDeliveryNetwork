using Microsoft.AspNetCore.Http;
using Minio;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IPictureService : IBaseDataService
    {
        Task<(byte[], string)> GetImage(string id);
        Task<bool> RemoveImage(string oldImageGuid);
        Task<PutObjectResponse> UploadImage(IFormFile image, string id);
    }
}
