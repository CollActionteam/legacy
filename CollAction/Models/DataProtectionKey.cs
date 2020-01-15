using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public sealed class DataProtectionKey
    {
        public DataProtectionKey(string friendlyName, string keyDataXml)
        {
            FriendlyName = friendlyName;
            KeyDataXml = keyDataXml;
        }

        [MaxLength(449)]
        [Required]
        [Key]
        public string FriendlyName { get; set; }

        [Required]
        public string KeyDataXml { get; set; }
    }
}
