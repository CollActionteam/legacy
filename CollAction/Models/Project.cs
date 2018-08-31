using CollAction.Data;
using Microsoft.EntityFrameworkCore;
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

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ProjectStatus Status { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        public Location Location { get; set; }

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

        public int? DescriptionVideoLinkId { get; set; }
        [ForeignKey("DescriptionVideoLinkId")]
        public VideoLink DescriptionVideoLink { get; set; }
        
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
        public List<ProjectParticipant> Participants { get; set; }
        public int AnonymousUserParticipants { get; set; }
        
        [NotMapped]
        public string HashTags
            => string.Join(";", Tags?.Select(tag => tag.Tag.Name) ?? Enumerable.Empty<string>());

        public async Task SetTags(ApplicationDbContext context, params string[] tagNames)
        {
            List<Tag> tags = await context.Tags.Where(tag => tagNames.Contains(tag.Name)).ToListAsync();
            IEnumerable<string> missingTags = tagNames.Where(tagName => !tags.Any(tag => tag.Name.Equals(tagName, StringComparison.Ordinal))).Distinct();
            if (missingTags.Any())
            {
                List<Tag> newTags = missingTags.Select(tagName => new Tag() { Name = tagName }).ToList();
                context.Tags.AddRange(newTags);
                tags.AddRange(newTags);
                await context.SaveChangesAsync();
            }

            List<ProjectTag> projectTags = await context.ProjectTags.Where(projectTag => projectTag.ProjectId == Id).ToListAsync();
            IEnumerable<ProjectTag> redundantTags = projectTags.Where(projectTag => !tags.Any(tag => tag.Id == projectTag.TagId));

            IEnumerable<ProjectTag> newProjectTags = tags.Where(tag => !projectTags.Any(projectTag => tag.Id == projectTag.TagId))
                                                         .Select(tag => new ProjectTag() { TagId = tag.Id, ProjectId = Id });

            if (redundantTags.Any() || newProjectTags.Any())
            {
                context.ProjectTags.RemoveRange(redundantTags);
                context.ProjectTags.AddRange(newProjectTags);
                await context.SaveChangesAsync();
            }
        }

        public void SetDescriptionVideoLink(ApplicationDbContext context, String videoLink)
        {
            // If the video link has changed...
            if (DescriptionVideoLink?.Link != videoLink)
            {
                // Remove the project's previously recorded video link if it exists.
                if (DescriptionVideoLink != null)
                {
                    context.VideoLinks.Remove(DescriptionVideoLink);
                }

                // If a new video link was specified add it to the VideoLinks table.
                if (videoLink != null)
                {
                    DescriptionVideoLink = new VideoLink
                    {
                        Link = videoLink,
                        Date = DateTime.UtcNow
                    };
                }
            }
        }
    }
}
