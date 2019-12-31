using CollAction.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Upload
{
    public class UploadImageViewModel
    {
        [MaxLength(50, ErrorMessage = "Keep your description short, no more then 50 characters")]
        [Display(Name = "Image description")]
        public string ImageDescription { get; set; }

        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        [Display(Name = "Description image")]
        public IFormFile Image { get; set; }
    }
}
