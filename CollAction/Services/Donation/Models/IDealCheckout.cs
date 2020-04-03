using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Donation.Models
{
    public sealed class IDealCheckout
    {
        [Required]
        public string SourceId { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;
    }
}
