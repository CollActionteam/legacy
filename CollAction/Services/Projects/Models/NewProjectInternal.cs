using CollAction.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollAction.Services.Projects.Models
{
    public class NewProjectInternal
    {
        public NewProjectInternal(string name, int target, string proposal, string description, string goal, string? creatorComments, DateTime start, DateTime end, int? cardImageFileId, int? bannerImageFileId, int? descriptiveImageFileId, string? descriptionVideoLink, IEnumerable<Category> categories, IEnumerable<string> tags, ProjectDisplayPriority displayPriority, ProjectStatus status, int anonymousUserParticipants, string? ownerId)
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

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

        public ProjectDisplayPriority DisplayPriority { get; set; }

        public ProjectStatus Status { get; set; }

        public int AnonymousUserParticipants { get; set; }

        public string? OwnerId { get; set; }
    }
}
