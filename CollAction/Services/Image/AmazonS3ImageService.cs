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
using CollAction.Helpers;
using SixLabors.ImageSharp.Processing;

namespace CollAction.Services.Image
{
    public class AmazonS3ImageService : IImageService
    {
        private readonly AmazonS3Client client;
        private readonly string bucket;
        private readonly string region;
        private readonly IBackgroundJobClient jobClient;
        private readonly int imageResizeThreshold;

        public AmazonS3ImageService(IOptions<ImageServiceOptions> options, IOptions<ImageProcessingOptions> processingOptions, IBackgroundJobClient jobClient)
        {
            client = new AmazonS3Client(options.Value.S3AwsAccessKeyID, options.Value.S3AwsAccessKey, RegionEndpoint.GetBySystemName(options.Value.S3Region));
            bucket = options.Value.S3Bucket;
            region = options.Value.S3Region;
            this.jobClient = jobClient;
            imageResizeThreshold = processingOptions.Value.MaxImageDimensionPixels;
        }

        public async Task<ImageFile> UploadImage(IFormFile fileUploaded, string imageDescription)
        {
            using (Image<Rgba32> image = await UploadToImage(fileUploaded))
            {
                var currentImage = new ImageFile()
                {
                    Filepath = $"{Guid.NewGuid()}.png"
                };

                currentImage.Date = DateTime.UtcNow;
                currentImage.Description = imageDescription;
                currentImage.Height = image.Height;
                currentImage.Width = image.Width;

                byte[] imageBytes = ConvertImageToPng(image);
                jobClient.Enqueue(() => 
                    UploadToS3(imageBytes, currentImage.Filepath));

                return currentImage;
            }
        }

        public void DeleteImage(ImageFile imageFile)
        {
            if (imageFile != null)
            {
                jobClient.Enqueue(() =>
                    DeleteObject(imageFile.Filepath));
            }
        }

        public string GetUrl(ImageFile imageFile)
            => $"https://{bucket}.s3.{region}.amazonaws.com/{imageFile.Filepath}";

        public async Task DeleteObject(string filePath)
        {
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = bucket,
                Key = filePath
            };

            DeleteObjectResponse response = await client.DeleteObjectAsync(deleteRequest);
            if (!response.HttpStatusCode.IsSuccess())
            {
                throw new InvalidOperationException($"failed to delete S3 object {filePath}, {response.HttpStatusCode}");
            }
        }

        public async Task UploadToS3(byte[] image, string path)
        {
            using (MemoryStream ms = new MemoryStream(image))
            {
                var putRequest = new PutObjectRequest()
                {
                    BucketName = bucket,
                    ContentType = "image/png",
                    Key = path,
                    InputStream = ms,
                    CannedACL = S3CannedACL.PublicRead,
                    AutoCloseStream = false
                };

                PutObjectResponse response = await client.PutObjectAsync(putRequest);
                if (!response.HttpStatusCode.IsSuccess())
                {
                    throw new InvalidOperationException($"failed to upload S3 object {path}, {response.HttpStatusCode}");
                }
            }
        }

        public async Task<Image<Rgba32>> UploadToImage(IFormFile fileUploaded)
        {
            using (Stream uploadStream = fileUploaded.OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await uploadStream.CopyToAsync(ms);
                    var image = SixLabors.ImageSharp.Image.Load(ms.ToArray());
                    var scaleRatio = GetScaleRatioForImage(image);
                    if (scaleRatio != 1.0)
                    {
                        image.Mutate(x => x
                            .Resize((int)(image.Width * scaleRatio), (int)(image.Height * scaleRatio)));
                    }

                    return image;
                }
            }
        }
    
        public void Dispose()
        {
            client.Dispose();
        }

        private byte[] ConvertImageToPng(Image<Rgba32> image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.SaveAsPng(ms);
                return ms.ToArray();
            }
        }

        private double GetScaleRatioForImage(Image<Rgba32> image)
        {
            if (image.Width > imageResizeThreshold || image.Height > imageResizeThreshold)
            {
                if (image.Width > image.Height) 
                {
                    return (double)imageResizeThreshold / image.Width;
                }
                else
                {
                    return (double)imageResizeThreshold / image.Height;
                }
            }

            return 1.0;
        }
    }
}
