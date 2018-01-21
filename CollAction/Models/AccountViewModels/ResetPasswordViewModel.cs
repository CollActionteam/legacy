using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        // Email
        [Required(ErrorMessage = "Het E-mailveld is verplicht.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

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

        public string Code { get; set; }
    }
}
