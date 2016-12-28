using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    /// <summary>
    /// County / District / 'Gemeente' etc
    /// </summary>
    public class LocationLevel2
    {
        [Key]
        [MaxLength(80)]
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
