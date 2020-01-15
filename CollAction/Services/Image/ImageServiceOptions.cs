using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Image
{
    public sealed class ImageServiceOptions
    {
        [Required]
        public string S3AwsAccessKeyID { get; set; } = null!;

        [Required]
        public string S3AwsAccessKey { get; set; } = null!;

        [Required]
        public string S3Bucket { get; set; } = null!;

        [Required]
        public string S3Region { get; set; } = null!;
    }
}
