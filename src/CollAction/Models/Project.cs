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
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        [Required]
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        [Required]
        public int Target { get; set; }

        [Required]
        public DateTime Start { get; set; }
        
        [Required]
        public DateTime End { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Description { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Goal { get; set; }
        
        [Required]
        [MaxLength(512)]
        public string Proposal { get; set; }

        [Required]
        [MaxLength(2048)]
        public string CreatorComments { get; set; }

        public int? BannerImageFileId { get; set; }
        [ForeignKey("BannerImageFileId")]
        public ImageFile BannerImage { get; set; }

        public ProjectDisplayPriority DisplayPriority { get; set; }

        [NotMapped]
        public bool IsActive
            => Status == ProjectStatus.Running && Start <= DateTime.UtcNow && End >= DateTime.UtcNow;

        [NotMapped]
        public bool IsComingSoon
            => Status == ProjectStatus.Running && Start > DateTime.UtcNow;

        [NotMapped]
        public bool IsClosed
            => Status == ProjectStatus.Running && End < DateTime.UtcNow;

        public List<ProjectTag> Tags { get; set; }
        public List<ProjectParticipant> Participants { get; set; }
        
        [NotMapped]
        public string HashTags
            => string.Join(";", Tags?.Select(tag => tag.Tag.Name) ?? Enumerable.Empty<string>());

        public async Task SetTags(ApplicationDbContext context, params string[] tagNames)
        {
            List<Tag> tags = await context.Tags.Where(tag => tagNames.Contains(tag.Name)).ToListAsync();
            IEnumerable<string> missingTags = tagNames.Where(tagName => !tags.Any(tag => tag.Name.Equals(tagName, StringComparison.Ordinal)));
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
    }
}
