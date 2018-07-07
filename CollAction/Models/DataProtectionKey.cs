using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
