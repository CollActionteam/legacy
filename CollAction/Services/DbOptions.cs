using System.ComponentModel.DataAnnotations;

namespace CollAction.Services
{
    public sealed class DbOptions
    {
        [Required]
        public string Db { get; set; } = null!;

        [Required]
        public string DbHost { get; set; } = null!;
        
        [Required]
        public string DbUser { get; set; } = null!;

        [Required]
        public string DbPassword { get; set; } = null!;

        [Required]
        public int DbPort { get; set; } = 5432;

        public string ConnectionString
            => $"Host={DbHost};Username={DbUser};Password={DbPassword};Database={Db};Port={DbPort}";
    }
}
