using CollAction.Services.Image;
using CollAction.ViewModels.Upload;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        private readonly IImageService imageService;

        public UploadController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(UploadImageViewModel uploadImage, CancellationToken cancellationToken)
        {
            var image = await imageService.UploadImage(uploadImage.ImageUpload, uploadImage.ImageDescription, cancellationToken);
            return Ok(image.Id);
        }
    }
}
