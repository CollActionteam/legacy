using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; internal set; }

        [Display(Name = "Last name")]
        public string LastName { get; internal set; }

        [Display(Name = "I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! 🙂")]
        public bool NewsletterSubscription { get; internal set; }
    }
}
