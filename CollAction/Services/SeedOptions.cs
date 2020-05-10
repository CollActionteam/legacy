using System.ComponentModel.DataAnnotations;

namespace CollAction.Services
{
    public sealed class SeedOptions
    {
        [Required]
        public string AdminEmail { get; set; } = null!;

        [Required]
        public string AdminPassword { get; set; } = null!;

        public bool SeedTestData { get; set; } = false;
    }
}
