using CollAction.Data;
using CollAction.Models;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.Extensions.Options;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CollAction.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace CollAction.Services.Image
{
    public class AmazonS3ImageService : IImageService
    {
        private readonly AmazonS3Client _client;
        private readonly string _bucket;
        private readonly string _region;
        private readonly IBackgroundJobClient _jobClient;

        public AmazonS3ImageService(IOptions<ImageServiceOptions> options, IBackgroundJobClient jobClient)
        {
            _client = new AmazonS3Client(options.Value.S3AwsAccessKeyID, options.Value.S3AwsAccessKey, RegionEndpoint.GetBySystemName(options.Value.S3Region));
            _bucket = options.Value.S3Bucket;
            _region = options.Value.S3Region;
            _jobClient = jobClient;
        }

        public async Task<ImageFile> UploadImage(ImageFile currentImage, IFormFile fileUploaded, string imageDescription)
        {
            using (Image<Rgba32> image = await UploadToImage(fileUploaded))
            {
                if (currentImage == null)
                {
                    currentImage = new ImageFile()
                    {
                        Filepath = $"{Guid.NewGuid()}.png"
                    };
                }

                currentImage.Date = DateTime.UtcNow;
                currentImage.Description = imageDescription;
                currentImage.Height = image.Height;
                currentImage.Width = image.Width;

                byte[] imageBytes = ConvertImageToPng(image);
                _jobClient.Enqueue(() => 
                    UploadToS3(imageBytes, currentImage.Filepath));

                return currentImage;
            }
        }

        public void DeleteImage(ImageFile imageFile)
        {
            if (imageFile != null)
                _jobClient.Enqueue(() => 
                    DeleteObject(imageFile.Filepath));
        }

        public string GetUrl(ImageFile imageFile)
            => $"https://s3.{_region}.amazonaws.com/{_bucket}/{imageFile.Filepath}";

        public async Task DeleteObject(string filePath)
        {
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = _bucket,
                Key = filePath
            };
            DeleteObjectResponse response = await _client.DeleteObjectAsync(deleteRequest);
            if (!response.HttpStatusCode.IsSuccess())
                throw new InvalidOperationException($"failed to delete S3 object {filePath}, {response.HttpStatusCode}");
        }

        public async Task UploadToS3(byte[] image, string path)
        {
            using (MemoryStream ms = new MemoryStream(image))
            {
                var putRequest = new PutObjectRequest()
                {
                    BucketName = _bucket,
                    ContentType = "image/png",
                    Key = path,
                    InputStream = ms,
                    CannedACL = S3CannedACL.PublicRead,
                    AutoCloseStream = false
                };

                PutObjectResponse response = await _client.PutObjectAsync(putRequest);
                if (!response.HttpStatusCode.IsSuccess())
                    throw new InvalidOperationException($"failed to upload S3 object {path}, {response.HttpStatusCode}");
            }
        }

        public async Task<Image<Rgba32>> UploadToImage(IFormFile fileUploaded)
        {
            using (Stream uploadStream = fileUploaded.OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await uploadStream.CopyToAsync(ms);
                    return SixLabors.ImageSharp.Image.Load(ms.ToArray());
                }
            }
        }

        private byte[] ConvertImageToPng(Image<Rgba32> image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.SaveAsPng(ms);
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
