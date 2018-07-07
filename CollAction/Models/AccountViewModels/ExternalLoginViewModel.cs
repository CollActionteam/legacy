using CollAction.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Display(Name = "Voornaam")]
        public string FirstName { get; internal set; }

        [Display(Name = "Achternaam")]
        public string LastName { get; internal set; }

        [Display(Name = "Ik ontvang graag af en toe een update van CollAction - geen zorgen, wij houden net zo weinig van spam als jij! 🙂")]
        public bool NewsletterSubscription { get; internal set; }

        [MustBeTrue(ErrorMessage = "Lees en accepteer aub ons privacy policy")]
        public bool AgreedPrivacyPolicy { get; set; }
    }
}
