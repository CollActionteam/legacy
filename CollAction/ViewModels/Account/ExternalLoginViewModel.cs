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
        public string ReturnUrl { get; set; } = null!;

        [Required]
        public string ErrorUrl { get; set; } = null!;
    }
}
