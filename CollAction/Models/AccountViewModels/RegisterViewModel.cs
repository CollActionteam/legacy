using CollAction.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Confirm email")]
        [Compare("Email", ErrorMessage = "The email and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! 🙂")]
        public bool NewsletterSubscription { get; set; }

        [MustBeTrue(ErrorMessage = "Please read and agree to the privacy agreement")]
        public bool AgreedPrivacyPolicy { get; set; }
    }
}
