using CollAction.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Viewmodel can't bind Uri")]
    public sealed class ExternalLoginCallbackViewModel
    {
        [Required]
        [FrontendUrl]
        public string ReturnUrl { get; set; } = null!;

        [Required]
        [FrontendUrl]
        public string ErrorUrl { get; set; } = null!;

        [Required]
        [FrontendUrl]
        public string FinishRegistrationUrl { get; set; } = null!;

        public string? RemoteError { get; set; }
    }
}
