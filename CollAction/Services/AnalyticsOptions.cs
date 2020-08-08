using System.ComponentModel.DataAnnotations;

namespace CollAction.Services
{
    public sealed class AnalyticsOptions
    {
        [Required]
        public string GoogleAnalyticsID { get; set; } = null!;

        [Required]
        public string FacebookPixelID { get; set; } = null!;
    }
}
