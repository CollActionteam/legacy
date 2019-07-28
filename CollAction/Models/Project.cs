using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CollAction.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ProjectStatus Status { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        public int Target { get; set; }

        public int NumberProjectEmailsSend { get; set; }

        public DateTime Start { get; set; }
        
        public DateTime End { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Goal { get; set; }

        [Required]
        [MaxLength(300)]
        public string Proposal { get; set; }

        [MaxLength(20000)]
        public string CreatorComments { get; set; }

        public int? BannerImageFileId { get; set; }
        [ForeignKey("BannerImageFileId")]
        public ImageFile BannerImage { get; set; }

        public int? DescriptiveImageFileId { get; set; }
        [ForeignKey("DescriptiveImageFileId")]
        public ImageFile DescriptiveImage { get; set; }

        public string DescriptionVideoLink { get; set; }
        
        public ProjectDisplayPriority DisplayPriority { get; set; }

        public ProjectParticipantCount ParticipantCounts { get; set; }

        [NotMapped]
        public bool IsActive
            => Status == ProjectStatus.Running && Start <= DateTime.UtcNow && End >= DateTime.UtcNow;

        [NotMapped]
        public bool IsComingSoon
            => Status == ProjectStatus.Running && Start > DateTime.UtcNow;

        [NotMapped]
        public bool IsClosed
            => Status == ProjectStatus.Running && End < DateTime.UtcNow;

        [NotMapped]
        public TimeSpan RemainingTime
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                return now >= End ? TimeSpan.Zero : End - (now > Start ? now : Start);
            }
        }

        public List<ProjectTag> Tags { get; set; }

        [NotMapped]
        public ProjectExternalStatus ExternalStatus
        {
            get
            {
                if (Status == ProjectStatus.Running && Start <= DateTime.UtcNow && End >= DateTime.UtcNow)
                    return ProjectExternalStatus.Open;
                else if (Status == ProjectStatus.Running && Start > DateTime.UtcNow)
                    return ProjectExternalStatus.ComingSoon;
                else
                    return ProjectExternalStatus.Closed;
            }
        }

        public List<ProjectParticipant> Participants { get; set; }

        public int AnonymousUserParticipants { get; set; }
    }
}
