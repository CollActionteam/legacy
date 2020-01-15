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
using System.Threading;
using Microsoft.Extensions.Logging;
using CollAction.Data;
using Microsoft.EntityFrameworkCore;
using Hangfire.Server;

namespace CollAction.Services.Image
{
    public sealed class AmazonS3ImageService : IImageService
    {
        private readonly AmazonS3Client client;
        private readonly string bucket;
        private readonly string region;
        private readonly IBackgroundJobClient jobClient;
        private readonly IRecurringJobManager recurringJobManager;
        private readonly ILogger<AmazonS3ImageService> logger;
        private readonly ApplicationDbContext context;
        private readonly int imageResizeThreshold;

        public AmazonS3ImageService(IOptions<ImageServiceOptions> options, IOptions<ImageProcessingOptions> processingOptions, IBackgroundJobClient jobClient, IRecurringJobManager recurringJobManager, ApplicationDbContext context, ILogger<AmazonS3ImageService> logger)
        {
            this.jobClient = jobClient;
            this.recurringJobManager = recurringJobManager;
            this.logger = logger;
            this.context = context;
            client = new AmazonS3Client(options.Value.S3AwsAccessKeyID, options.Value.S3AwsAccessKey, RegionEndpoint.GetBySystemName(options.Value.S3Region));
            bucket = options.Value.S3Bucket;
            region = options.Value.S3Region;
            imageResizeThreshold = processingOptions.Value.MaxImageDimensionPixels;
        }

        public async Task<ImageFile> UploadImage(IFormFile fileUploaded, string imageDescription, CancellationToken token)
        {
            logger.LogInformation("Uploading image");
            using Image<Rgba32> image = await UploadToImage(fileUploaded, token);
            var currentImage = new ImageFile()
            {
                Filepath = $"{Guid.NewGuid()}.png",
                Date = DateTime.UtcNow,
                Description = imageDescription,
                Height = image.Height,
                Width = image.Width
            };

            logger.LogInformation("Queuing for s3 upload");
            byte[] imageBytes = ConvertImageToPng(image);
            jobClient.Enqueue(() => UploadToS3(imageBytes, currentImage.Filepath));

            logger.LogInformation("Saving image information to database");
            context.ImageFiles.Add(currentImage);
            await context.SaveChangesAsync(token);

            logger.LogInformation("Done uploading image");

            return currentImage;
        }

        public async Task DeleteImage(ImageFile imageFile, CancellationToken token)
        {
            if (imageFile != null)
            {
                logger.LogInformation("Deleting image");
                context.ImageFiles.Remove(imageFile);
                await context.SaveChangesAsync(token);
                
                logger.LogInformation("Queueing image removal from s3");
                jobClient.Enqueue(() => DeleteObject(imageFile.Filepath, token));
            }
        }

        public string GetUrl(ImageFile imageFile)
            => $"https://{bucket}.s3.{region}.amazonaws.com/{imageFile.Filepath}";

        public async Task DeleteObject(string filePath, CancellationToken token)
        {
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = bucket,
                Key = filePath
            };

            logger.LogInformation("Deleting image from s3");
            DeleteObjectResponse response = await client.DeleteObjectAsync(deleteRequest, token);
            if (!response.HttpStatusCode.IsSuccess())
            {
                logger.LogError("Error removing image from s3");
                throw new InvalidOperationException($"failed to delete S3 object {filePath}, {response.HttpStatusCode}");
            }

            logger.LogInformation("Successfully removed image from s3");
        }

        public async Task UploadToS3(byte[] image, string path)
        {
            logger.LogInformation("Adding image to s3");
            using MemoryStream ms = new MemoryStream(image);
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
                logger.LogError("Error uploading image to s3");
                throw new InvalidOperationException($"failed to upload S3 object {path}, {response.HttpStatusCode}");
            }

            logger.LogInformation("Successfully added image to s3");
        }

        public async Task RemoveDanglingImages(PerformContext performContext, CancellationToken token)
        {
            // Fetch images without projects older than 1 hour
            var danglingImages = await context.ImageFiles.FromSqlRaw(
                @"SELECT *
                  FROM public.""ImageFiles"" I
                  WHERE NOT EXISTS
                  (
                      SELECT NULL
                      FROM public.""Projects"" P
                      WHERE P.""BannerImageFileId"" = I.""Id"" OR P.""DescriptiveImageFileId"" = I.""Id""
                  ) AND I.""Date"" < 'now'::timestamp - '1 hour'::interval").ToListAsync();

            danglingImages.ForEach(i => jobClient.ContinueJobWith(performContext.BackgroundJob.Id, () => DeleteObject(i.Filepath, token)));

            context.ImageFiles.RemoveRange(danglingImages);
            await context.SaveChangesAsync();
        }

        public void InitializeDanglingImageJob()
        {
            recurringJobManager.AddOrUpdate("dangling-image-job", () => RemoveDanglingImages(null!, CancellationToken.None), Cron.Hourly);
        }
    
        public void Dispose()
        {
            client.Dispose();
        }

        private async Task<Image<Rgba32>> UploadToImage(IFormFile fileUploaded, CancellationToken token)
        {
            using Stream uploadStream = fileUploaded.OpenReadStream();
            using MemoryStream ms = new MemoryStream();
            await uploadStream.CopyToAsync(ms, token);
            var image = SixLabors.ImageSharp.Image.Load(ms.ToArray());
            var scaleRatio = GetScaleRatioForImage(image);
            if (scaleRatio != 1.0)
            {
                image.Mutate(x => x
                     .Resize((int)(image.Width * scaleRatio), (int)(image.Height * scaleRatio)));
            }

            return image;
        }

        private byte[] ConvertImageToPng(Image<Rgba32> image)
        {
            using MemoryStream ms = new MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
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
