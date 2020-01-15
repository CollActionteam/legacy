using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    public class ExternalLoginViewModel
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
