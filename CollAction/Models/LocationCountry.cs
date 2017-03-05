using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class LocationCountry
    {
        [Key]
        [MaxLength(2)]
        public string Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string CapitalCity { get; set; }

        [Required]
        public int LocationId { get; set; }
        public Location Location { get; set; }

        [Required]
        [MaxLength(2)]
        public string ContinentId { get; set; }
        public LocationContinent Continent { get; set; }

        public List<Location> Locations { get; set; }
    }
}
