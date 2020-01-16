using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    public sealed class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }

        public string? ErrorUrl { get; set; }
    }
}
