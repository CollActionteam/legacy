using System.ComponentModel.DataAnnotations;

namespace CollAction.Services
{
    public class SiteOptions
    {
        [Required]
        public string PublicAddress { get; set; } = null!;
    }
}
