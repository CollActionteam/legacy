using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        // TODO: store with postgis
        [Column(TypeName = "numeric(13,10)")]
        public decimal Longitude { get; set; }

        // TODO: store with postgis
        [Column(TypeName = "numeric(13,10)")]
        public decimal Latitude { get; set; }

        public LocationFeatureClass FeatureClass { get; set; }
        public LocationFeature Feature { get; set; }

        [MaxLength(2)]
        public string CountryId { get; set; }
        public LocationCountry Country { get; set; }       

        [Required]
        [MaxLength(40)]
        public string TimeZone { get; set; }

        [MaxLength(20)]
        public string Level1Id { get; set; }
        public LocationLevel1 Level1 { get; set; }

        [MaxLength(80)]
        public string Level2Id { get; set; }
        public LocationLevel2 Level2 { get; set; }

        public List<LocationAlternateName> AlternateNames { get; set; }
    }
}
