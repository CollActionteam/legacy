using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    public sealed class ExternalLoginCallbackViewModel
    {
        [Required]
        public string ReturnUrl { get; set; } = null!;

        [Required]
        public string ErrorUrl { get; set; } = null!;

        public string? RemoteError { get; set; }
    }
}
