using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.User.Models
{
    public sealed class UpdatedUser
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int representsNumberParticipants { get; set; }

        public bool IsSubscribedNewsletter { get; set; }

        public bool IsAdmin { get; set; }
    }
}
