using CollAction.Models;
using CollAction.Services.Image;
using CollAction.ViewModels.Upload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("image")]
    [ApiController]
    public sealed class ImageController : Controller
    {
        private readonly IImageService imageService;
        private readonly ILogger<ImageController> logger;

        public ImageController(IImageService imageService, ILogger<ImageController> logger)
        {
            this.imageService = imageService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageViewModel uploadImage, CancellationToken token)
        {
            try
            {
                ImageFile image = await imageService.UploadImage(uploadImage.Image, uploadImage.ImageDescription, uploadImage.ImageResizeThreshold, token).ConfigureAwait(false);
                return Ok(new Dictionary<string, int>() { { "id", image.Id } });
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException e) 
            {
                logger.LogError(e, "Error uploading image");
                return StatusCode(StatusCodes.Status400BadRequest, new Dictionary<string, string>() { { "errorMessage", "Invalid image format. Use GIF, PNG, JPEG or BMP" } });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error uploading image");
                return StatusCode(StatusCodes.Status500InternalServerError, new Dictionary<string, string>() { { "errorMessage", e.Message } });
            }
        }
    }
}
