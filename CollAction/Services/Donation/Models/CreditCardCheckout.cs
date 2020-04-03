using CollAction.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Donation.Models
{
#pragma warning disable CA1056 // Uri properties should not be strings
    public sealed class CreditCardCheckout
    {
        [Required]
        public string Currency { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public int Amount { get; set; }

        public bool Recurring { get; set; }

        [Required]
        [FrontendUrl]
        public string SuccessUrl { get; set; } = null!;

        [Required]
        [FrontendUrl]
        public string CancelUrl { get; set; } = null!;
    }
#pragma warning restore CA1056 // Uri properties should not be strings
}
