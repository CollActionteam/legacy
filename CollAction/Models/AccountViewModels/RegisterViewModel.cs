using CollAction.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        // Email
        [Required(ErrorMessage = "Het e-mailveld is verplicht.")]
        [EmailAddress(ErrorMessage = "Het e-mailveld is geen geldig e-mailadres.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        // Confirm email
        [Display(Name = "Bevestig e-mail")]
        // The email and confirmation email do not match. 
        [Compare("Email", ErrorMessage = "Je e-mail en bevestigingsmail komen niet overeen.")]
        public string ConfirmEmail { get; set; }

        // Password
        [Required(ErrorMessage = "Het wachtwoordveld is verplicht.")]
        // The {0} must be at least {2} and at max {1} characters long.
        [StringLength(100, ErrorMessage = "Het {0} moet minimaal {2} en maximaal {1} tekens bevatten.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        // Confirm password
        [DataType(DataType.Password)]
        [Display(Name = "Bevestig wachtwoord")]
        // The password and confirmation password do not match.
        [Compare("Password", ErrorMessage = "Wachtwoord en bevesting wachtwoord komen niet overeen.")]
        public string ConfirmPassword { get; set; }

        // First name
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        // Last name 
        [Display(Name = "Achternaam")]
        public string LastName { get; set; }

        // I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! 🙂
        [Display(Name = "Ik ontvang graag af en toe een update van CollAction - geen zorgen, wij houden net zo weinig van spam als jij! 🙂")]
        public bool NewsletterSubscription { get; set; }

        [MustBeTrue(ErrorMessage = "Lees en accepteer aub ons privacy policy")]
        public bool AgreedPrivacyPolicy { get; set; }
    }
}
