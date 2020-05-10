using CollAction.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CollAction.Models
{
    public sealed class Crowdaction
    {
        public Crowdaction(string name, CrowdactionStatus status, string? ownerId, int target, DateTime start, DateTime end, string description, string goal, string proposal, string? creatorComments, string? descriptionVideoLink, CrowdactionDisplayPriority displayPriority = CrowdactionDisplayPriority.Medium, int? bannerImageFileId = null, int? descriptiveImageFileId = null, int? cardImageFileId = null, int anonymousUserParticipants = 0) : this(name, status, ownerId, target, start, end, description, goal, proposal, creatorComments, descriptionVideoLink, new List<CrowdactionCategory>(), new List<CrowdactionTag>(), displayPriority, bannerImageFileId, descriptiveImageFileId, cardImageFileId, anonymousUserParticipants)
        {
        }

        public Crowdaction(string name, CrowdactionStatus status, string? ownerId, int target, DateTime start, DateTime end, string description, string goal, string proposal, string? creatorComments, string? descriptionVideoLink, ICollection<CrowdactionCategory> categories, ICollection<CrowdactionTag> tags, CrowdactionDisplayPriority displayPriority = CrowdactionDisplayPriority.Medium, int? bannerImageFileId = null, int? descriptiveImageFileId = null, int? cardImageFileId = null, int anonymousUserParticipants = 0)
        {
            Name = name;
            Status = status;
            OwnerId = ownerId;
            Target = target;
            Start = start;
            End = end;
            Description = description;
            Goal = goal;
            Proposal = proposal;
            CreatorComments = creatorComments;
            DescriptionVideoLink = descriptionVideoLink;
            DisplayPriority = displayPriority;
            BannerImageFileId = bannerImageFileId;
            CardImageFileId = cardImageFileId;
            DescriptiveImageFileId = descriptiveImageFileId;
            Categories = categories;
            Tags = tags;
            AnonymousUserParticipants = anonymousUserParticipants;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public CrowdactionStatus Status { get; set; }

        public string? OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }

        public int Target { get; set; }

        public int NumberCrowdactionEmailsSent { get; set; }

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
        public string? CreatorComments { get; set; }

        public string? DescriptionVideoLink { get; set; }

        public CrowdactionDisplayPriority DisplayPriority { get; set; }

        [MaxLength(100)]
        public string? FinishJobId { get; set; }

        public int AnonymousUserParticipants { get; set; }

        public int? BannerImageFileId { get; set; }

        [ForeignKey("BannerImageFileId")]
        public ImageFile? BannerImage { get; set; }

        public int? DescriptiveImageFileId { get; set; }

        [ForeignKey("DescriptiveImageFileId")]
        public ImageFile? DescriptiveImage { get; set; }

        public int? CardImageFileId { get; set; }

        [ForeignKey("CardImageFileId")]
        public ImageFile? CardImage { get; set; }

        public CrowdactionParticipantCount? ParticipantCounts { get; set; }

        public ICollection<CrowdactionParticipant> Participants { get; set; } = new List<CrowdactionParticipant>();

        public ICollection<CrowdactionCategory> Categories { get; set; }

        public ICollection<CrowdactionTag> Tags { get; set; }

        [NotMapped]
        public bool IsActive
            => Status == CrowdactionStatus.Running && Start <= DateTime.UtcNow && End >= DateTime.UtcNow;

        [NotMapped]
        public bool IsComingSoon
            => Status == CrowdactionStatus.Running && Start > DateTime.UtcNow;

        [NotMapped]
        public bool IsClosed
            => Status == CrowdactionStatus.Running && End < DateTime.UtcNow;

        [NotMapped]
        public int TotalParticipants
            => (ParticipantCounts?.Count ?? throw new InvalidOperationException("ParticipantCounts not available")) + AnonymousUserParticipants;

        [NotMapped]
        public int Percentage
            => (int)Math.Round(100 * (double)TotalParticipants / Target, 0);

        [NotMapped]
        public bool IsSuccessfull
            => IsClosed &&
               TotalParticipants >= Target;

        [NotMapped]
        public bool IsFailed
            => IsClosed &&
               TotalParticipants < Target;

        [NotMapped]
        public string NameNormalized
            => ToUrlSlug(RemoveDiacriticsFromString(Name));

        [NotMapped]
        public Uri Url
            => new Uri($"/crowdactions/{NameNormalized}/{Id}", UriKind.Relative);

        [NotMapped]
        public string RemainingTimeUserFriendly
        {
            get
            {
                TimeSpan remaining = RemainingTime;
                if (remaining.Years() > 1)
                {
                    return $"{remaining.Years()} years";
                }
                else if (remaining.Months() > 1)
                {
                    return $"{remaining.Months()} months";
                }
                else if (remaining.Weeks() > 1)
                {
                    return $"{remaining.Weeks()} weeks";
                }
                else if (remaining.Days > 1)
                {
                    return $"{remaining.Days} days";
                }
                else if (remaining.Hours > 1)
                {
                    return $"{(int)remaining.TotalHours} hours";
                }
                else if (remaining.Minutes > 0)
                {
                    return $"{remaining.Minutes} minutes";
                }
                else if (IsSuccessfull)
                {
                    return "Successfull";
                }
                else if (IsFailed)
                {
                    return "Failed";
                }
                else
                {
                    return "Done";
                }
            }
        }

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
            value = invalidCharRemoveRegex.Replace(value, string.Empty);
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
