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

        public int NumberSeededCrowdactions { get; set; } = 40;

        public int NumberSeededTags { get; set; } = 30;

        public int NumberDaysSeededForComments { get; set; } = 30;

        public double ProbabilityCommentSeededPerHour { get; set; } = 0.05;
    }
}
