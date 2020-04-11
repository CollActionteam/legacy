using System.ComponentModel.DataAnnotations;

namespace CollAction.Services
{
    public sealed class DisqusOptions
    {
        [Required]
        public string DisqusSiteId { get; set; } = null!;
    }
}
