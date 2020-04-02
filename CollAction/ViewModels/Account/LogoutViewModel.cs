using CollAction.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.Account
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Viewmodel can't bind Uri")]
    public class LogoutViewModel
    {
        [IsFrontendUrl]
        [Required]
        public string ReturnUrl { get; set; } = null!;
    }
}
