using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.User.Models
{
    public sealed class NewUser
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool IsSubscribedNewsletter { get; set; }
    }
}
