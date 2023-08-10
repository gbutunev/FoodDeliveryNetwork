using FoodDeliveryNetwork.Services.Data.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;

namespace FoodDeliveryNetwork.Services.Data
{
    public class PictureService : IPictureService
    {
        private readonly MinioClient minioClient;
        private readonly string bucketName;

        public PictureService(IConfiguration configuration)
        {
            var endpoint = configuration["MinIOConfig:Endpoint"];
            var accessKey = configuration["MinIOConfig:AccessKey"];
            var secretKey = configuration["MinIOConfig:SecretKey"];
            bucketName = configuration["MinIOConfig:BucketName"];

            minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build();

        }

        public async Task<PutObjectResponse> UploadImage(IFormFile image, string id)
        {
            using var stream = image.OpenReadStream();
            PutObjectArgs args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(id)
                .WithContentType(image.ContentType)
                .WithObjectSize(stream.Length)
                .WithStreamData(stream);

            return await minioClient.PutObjectAsync(args);
        }

        public async Task<(byte[], string)> GetImage(string id)
        {
            using var stream = new MemoryStream();
            var contentType = string.Empty;

            GetObjectArgs args = new GetObjectArgs()
              .WithBucket(bucketName)
              .WithObject(id)
              .WithCallbackStream((callBackResponse) =>
              {
                  callBackResponse.CopyTo(stream);
              });

            try
            {
                var r = await minioClient.GetObjectAsync(args);
                return (stream.ToArray(), r.ContentType);
            }
            catch (Exception)
            {
                return (null, null);
            }

        }

        public async Task<bool> RemoveImage(string oldImageGuid)
        {
            RemoveObjectArgs args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(oldImageGuid);
            try
            {
                await minioClient.RemoveObjectAsync(args);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
