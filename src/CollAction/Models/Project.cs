using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        [MinLength(6)]
        public string Name { get; set; }

        [MaxLength(256)]
        [MinLength(6)]
        public string Title { get; set; }

        [MaxLength(1024)]
        [MinLength(12)]
        public string ShortDescription { get; set; }

        [MinLength(12)]
        public string Description { get; set; }

        public int Target { get; set; }
        public DateTime Deadline { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}
