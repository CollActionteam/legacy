using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    public class ExternalLoginCallbackViewModel
    {
        [Required]
        public string ReturnUrl { get; set; }

        [Required]
        public string ErrorUrl { get; set; }

        public string? RemoteError { get; set; }
    }
}
