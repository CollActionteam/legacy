using CollAction.Models;
using CollAction.Services.Image;
using CollAction.ViewModels.Upload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("image")]
    [ApiController]
    public sealed class ImageController : Controller
    {
        private readonly IImageService imageService;

        public ImageController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageViewModel uploadImage, CancellationToken token)
        {
            ImageFile image = await imageService.UploadImage(uploadImage.Image, uploadImage.ImageDescription, token);
            imageService.InitializeDanglingImageJob();
            return Ok(image.Id);
        }
    }
}
