using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.Services
{
    public sealed class SiteOptions
    {
        [Required]
        public string PublicAddress { get; set; } = null!;

        public IEnumerable<string> PublicAddresses
            => PublicAddress.Split(";");

        public string CanonicalAddress
            => PublicAddresses.First();
    }
}
