using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public sealed class DataProtectionKey
    {
        [MaxLength(449)]
        [Required]
        [Key]
        public string FriendlyName { get; set; }

        [Required]
        public string KeyDataXml { get; set; }
    }
}
