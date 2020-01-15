using System.ComponentModel.DataAnnotations;

namespace CollAction.Services
{
    public class SeedOptions
    {
        [Required]
        public string AdminEmail { get; set; } = null!;
        
        [Required]
        public string AdminPassword { get; set; } = null!;

        public bool SeedTestProjects { get; set; } = false;
    }
}
