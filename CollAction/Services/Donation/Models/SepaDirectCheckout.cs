using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Donation.Models
{
    public sealed class SepaDirectCheckout
    {
        [Required]
        public string SourceId { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public int Amount { get; set; }
    }
}
