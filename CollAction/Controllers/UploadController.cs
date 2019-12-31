using CollAction.Services.Image;
using CollAction.ViewModels.Upload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("upload")]
    [ApiController]
    public class UploadController : Controller
    {
        private readonly IImageService imageService;

        public UploadController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageViewModel uploadImage, CancellationToken cancellationToken)
        {
            var image = await imageService.UploadImage(uploadImage.Image, uploadImage.ImageDescription, cancellationToken);
            return Ok(image.Id);
        }
    }
}
