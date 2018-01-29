using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Het E-mailveld is verplicht.")]
        [EmailAddress(ErrorMessage = "Het e-mailveld is geen geldig e-mailadres.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
