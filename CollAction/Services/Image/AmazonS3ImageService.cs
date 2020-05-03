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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace CollAction.Services.Image
{
    public sealed class AmazonS3ImageService : IImageService
    {
        private readonly AmazonS3Client client;
        private readonly string bucket;
        private readonly string region;
        private readonly JpegEncoder encoder;
        private readonly IRecurringJobManager recurringJobManager;
        private readonly ILogger<AmazonS3ImageService> logger;
        private readonly ApplicationDbContext context;

        public AmazonS3ImageService(IOptions<ImageServiceOptions> options, IRecurringJobManager recurringJobManager, ApplicationDbContext context, ILogger<AmazonS3ImageService> logger)
        {
            this.recurringJobManager = recurringJobManager;
            this.logger = logger;
            this.context = context;
            client = new AmazonS3Client(options.Value.S3AwsAccessKeyID, options.Value.S3AwsAccessKey, RegionEndpoint.GetBySystemName(options.Value.S3Region));
            bucket = options.Value.S3Bucket;
            region = options.Value.S3Region;
            encoder = new JpegEncoder() { Quality = 90 };
        }

        public async Task<ImageFile> UploadImage(IFormFile fileUploaded, string imageDescription, int imageResizeThreshold, CancellationToken token)
        {
            logger.LogInformation("Retrieving image");
            using Image<Rgba32> image = await LoadImageFromRequest(fileUploaded, imageResizeThreshold, token).ConfigureAwait(false);
            var currentImage = new ImageFile(filepath: $"{Guid.NewGuid()}.jpg", date: DateTime.UtcNow, description: imageDescription, height: image.Height, width: image.Width);

            logger.LogInformation("Uploading image to S3");
            using MemoryStream imageBytes = ConvertImageToJpg(image);
            await UploadToS3(imageBytes, currentImage.Filepath, token).ConfigureAwait(false);

            logger.LogInformation("Saving image information to database");
            context.ImageFiles.Add(currentImage);
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            logger.LogInformation("Done processing image");

            return currentImage;
        }

        public async Task DeleteImage(ImageFile? imageFile, CancellationToken token)
        {
            if (imageFile != null)
            {
                logger.LogInformation("Deleting image");
                context.ImageFiles.Remove(imageFile);
                await context.SaveChangesAsync(token).ConfigureAwait(false);
                
                logger.LogInformation("Removing image from S3");
                await DeleteObject(imageFile.Filepath, token).ConfigureAwait(false);
            }
        }

        public Uri GetUrl(ImageFile imageFile)
            => new Uri($"https://{bucket}.s3.{region}.amazonaws.com/{imageFile.Filepath}");

        public async Task DeleteObject(string filePath, CancellationToken token)
        {
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = bucket,
                Key = filePath
            };

            logger.LogInformation("Deleting image from s3");
            DeleteObjectResponse response = await client.DeleteObjectAsync(deleteRequest, token).ConfigureAwait(false);
            if (!response.HttpStatusCode.IsSuccess())
            {
                logger.LogError("Error removing image from s3");
                throw new InvalidOperationException($"failed to delete S3 object {filePath}, {response.HttpStatusCode}");
            }

            logger.LogInformation("Successfully removed image from s3");
        }

        public async Task UploadToS3(MemoryStream image, string path, CancellationToken cancellationToken)
        {
            logger.LogInformation("Adding image to s3");
            var putRequest = new PutObjectRequest()
            {
                BucketName = bucket,
                ContentType = "image/jpg",
                Key = path,
                InputStream = image,
                CannedACL = S3CannedACL.PublicRead,
                AutoCloseStream = false
            };

            putRequest.Headers.CacheControl = "max-age=31556952"; // We don't change these images, one year caching header

            PutObjectResponse response = await client.PutObjectAsync(putRequest, cancellationToken).ConfigureAwait(false);
            if (!response.HttpStatusCode.IsSuccess())
            {
                logger.LogError("Error uploading image to s3");
                throw new InvalidOperationException($"failed to upload S3 object {path}, {response.HttpStatusCode}");
            }

            logger.LogInformation("Successfully added image to s3");
        }

        public async Task RemoveDanglingImages(CancellationToken token)
        {
            // Fetch images without projects older than 1 hour
            var danglingImages = await context.ImageFiles.FromSqlRaw(
                @"SELECT *
                  FROM public.""ImageFiles"" I
                  WHERE NOT EXISTS
                  (
                      SELECT NULL
                      FROM public.""Projects"" P
                      WHERE P.""BannerImageFileId"" = I.""Id"" OR P.""DescriptiveImageFileId"" = I.""Id"" OR P.""CardImageFileId"" = I.""Id""
                  ) AND I.""Date"" < 'now'::timestamp - '1 hour'::interval").ToListAsync().ConfigureAwait(false);

            await Task.WhenAll(danglingImages.Select(i => DeleteObject(i.Filepath, token))).ConfigureAwait(false);

            context.ImageFiles.RemoveRange(danglingImages);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void InitializeDanglingImageJob()
        {
            recurringJobManager.AddOrUpdate("dangling-image-job", () => RemoveDanglingImages(CancellationToken.None), Cron.Hourly);
        }
    
        public void Dispose()
        {
            client.Dispose();
        }

        private static double GetScaleRatioForImage(Image<Rgba32> image, int imageResizeThreshold)
        {
            if (imageResizeThreshold < 1)
            {
                throw new ValidationException($"Invalid image resize threshold: {imageResizeThreshold}");
            }

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

        private async Task<Image<Rgba32>> LoadImageFromRequest(IFormFile fileUploaded, int imageResizeThreshold, CancellationToken token)
        {
            using Stream uploadStream = fileUploaded.OpenReadStream();
            using MemoryStream ms = new MemoryStream();
            await uploadStream.CopyToAsync(ms, token).ConfigureAwait(false);
            var image = SixLabors.ImageSharp.Image.Load(ms.ToArray());
            var scaleRatio = GetScaleRatioForImage(image, imageResizeThreshold);
            if (scaleRatio != 1.0)
            {
                image.Mutate(x => x
                     .Resize((int)(image.Width * scaleRatio), (int)(image.Height * scaleRatio)));
            }

            return image;
        }

        private MemoryStream ConvertImageToJpg(Image<Rgba32> image)
        {
            MemoryStream ms = new MemoryStream();
            image.SaveAsJpeg(ms, encoder);
            return ms;
        }
    }
}
