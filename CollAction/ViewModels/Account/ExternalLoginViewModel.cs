using CollAction.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Viewmodel can't bind Uri")]
    public sealed class ExternalLoginViewModel
    {
        [Required]
        public string Provider { get; set; } = null!;

        public bool RememberMe { get; set; }

        [Required]
        [FrontendUrl]
        public string ReturnUrl { get; set; } = null!;

        [Required]
        [FrontendUrl]
        public string ErrorUrl { get; set; } = null!;
    }
}
