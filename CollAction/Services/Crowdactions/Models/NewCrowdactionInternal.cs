using CollAction.Migrations;
using CollAction.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollAction.Services.Crowdactions.Models
{
    public sealed class NewCrowdactionInternal
    {
        public NewCrowdactionInternal(string name, int target, string proposal, string description, string goal, string? creatorComments, DateTime start, DateTime end, string? instagramUser, int? cardImageFileId, int? bannerImageFileId, int? descriptiveImageFileId, string? descriptionVideoLink, IEnumerable<Category> categories, IEnumerable<string> tags, CrowdactionDisplayPriority displayPriority, CrowdactionStatus status, int anonymousUserParticipants, string? ownerId)
        {
            Name = name;
            Target = target;
            Proposal = proposal;
            Description = description;
            Goal = goal;
            CreatorComments = creatorComments;
            Start = start;
            End = end;
            CardImageFileId = cardImageFileId;
            BannerImageFileId = bannerImageFileId;
            DescriptiveImageFileId = descriptiveImageFileId;
            DescriptionVideoLink = descriptionVideoLink;
            InstagramUser = instagramUser;
            Categories = categories;
            Tags = tags;
            DisplayPriority = displayPriority;
            Status = status;
            AnonymousUserParticipants = anonymousUserParticipants;
            OwnerId = ownerId;
        }

        public string Name { get; set; }

        public int Target { get; set; }

        public string Proposal { get; set; }

        public string Description { get; set; }

        public string Goal { get; set; }

        public string? CreatorComments { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int? CardImageFileId { get; set; }

        public int? BannerImageFileId { get; set; }

        public int? DescriptiveImageFileId { get; set; }

        public string? DescriptionVideoLink { get; set; }

        public string? InstagramUser { get; }

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

        public CrowdactionDisplayPriority DisplayPriority { get; set; }

        public CrowdactionStatus Status { get; set; }

        public int AnonymousUserParticipants { get; set; }

        public string? OwnerId { get; set; }
    }
}
