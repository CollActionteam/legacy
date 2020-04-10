using System;
using System.Threading;
using System.Threading.Tasks;
using CollAction.Models;
using Microsoft.AspNetCore.Http;

namespace CollAction.Services.Image
{
    public interface IImageService : IDisposable
    {
        Task<ImageFile> UploadImage(IFormFile fileUploaded, string imageDescription, CancellationToken token);

        Task DeleteImage(ImageFile imageFile, CancellationToken token);

        Uri GetUrl(ImageFile imageFile);

        // Removes images that have no associated project, to prevent costs in our S3 bucket
        void InitializeDanglingImageJob();
    }
}