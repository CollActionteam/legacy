using CollAction.Services.Image;
using CollAction.ViewModels.Upload;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> UploadImage(UploadImageViewModel uploadImage)
        {
            var image = await imageService.UploadImage(null, uploadImage.ImageUpload, uploadImage.ImageDescription);
            return Ok(image.Id);
        }
    }
}
