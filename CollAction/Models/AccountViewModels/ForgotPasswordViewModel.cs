using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        // Email
        [Required(ErrorMessage = "Het e-mailveld is verplicht.")]
        [EmailAddress(ErrorMessage = "Het e-mailveld is geen geldig e-mailadres.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
