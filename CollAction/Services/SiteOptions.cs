using System.ComponentModel.DataAnnotations;

namespace CollAction.Services
{
    public sealed class SiteOptions
    {
        [Required]
        [RegularExpression(@"^.*[^\\]$", ErrorMessage = "PublicAddress must not end with a slash")] // Does not end with a slash
        public string PublicAddress { get; set; } = null!;
    }
}
