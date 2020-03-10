using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    public class LogoutViewModel
    {
        [Required]
        public string ReturnUrl { get; set; }
    }
}
