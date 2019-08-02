using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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

        public int AnonymousUserParticipants { get; set; }

        public int? BannerImageFileId { get; set; }
        [ForeignKey("BannerImageFileId")]
        public ImageFile BannerImage { get; set; }

        public int? DescriptiveImageFileId { get; set; }
        [ForeignKey("DescriptiveImageFileId")]
        public ImageFile DescriptiveImage { get; set; }

        public string DescriptionVideoLink { get; set; }
        
        public ProjectDisplayPriority DisplayPriority { get; set; }

        public ProjectParticipantCount ParticipantCounts { get; set; }

        public List<ProjectParticipant> Participants { get; set; }

        public List<ProjectTag> Tags { get; set; }

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
        public string NameNormalized
            => ToUrlSlug(RemoveDiacriticsFromString(Name));

        [NotMapped]
        public string Url
            => $"/#/Projects/{NameNormalized}/{Id}";

        [NotMapped]
        public TimeSpan RemainingTime
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                if (now >= End)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    return End - (now > Start ? now : Start);
                }
            }
        }

        private static string ToUrlSlug(string value)
        {
            Regex spaceRemoveRegex = new Regex(@"\s");
            Regex invalidCharRemoveRegex = new Regex(@"[^a-z0-9\s-_]");
            Regex doubleDashRemoveRegex = new Regex(@"([-_]){2,}");
            value = value.ToLowerInvariant();
            value = spaceRemoveRegex.Replace(value, "-");
            value = invalidCharRemoveRegex.Replace(value, "");
            value = value.Trim('-', '_');
            value = doubleDashRemoveRegex.Replace(value, "$1");
            if (value.Length == 0)
            {
                value = "-";
            }

            return value;
        }

        private static string RemoveDiacriticsFromString(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
