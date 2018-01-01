using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        // Email
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        // Confirm email
        [Display(Name = "Bevestig e-mail")]
        [Compare("Email", ErrorMessage = "The email and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }

        // Password
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        // Confirm password
        [DataType(DataType.Password)]
        [Display(Name = "Bevestig wachtwoord")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // First name
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        // Last name 
        [Display(Name = "Achternaam")]
        public string LastName { get; set; }

        // I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! 🙂
        [Display(Name = "Ik ontvang graag af en toe een update van CollAction (geen zorgen, wij houden net zo weinig van spam als jij! 🙂")]
        public bool NewsletterSubscription { get; set; }
    }
}
