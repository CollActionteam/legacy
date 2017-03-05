using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    /// <summary>
    /// Province / State
    /// </summary>
    public class LocationLevel1
    {
        [Key]
        [MaxLength(20)]
        public string Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        public List<Location> Locations { get; set; }
    }
}
