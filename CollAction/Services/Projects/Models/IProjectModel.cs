using System;
using System.Collections.Generic;
using CollAction.Models;

namespace CollAction.Services.Projects.Models
{
    public interface IProjectModel
    {
        int? BannerImageFileId { get; set; }

        ICollection<Category> Categories { get; set; }

        string CreatorComments { get; set; }

        string Description { get; set; }

        string DescriptionVideoLink { get; set; }

        int? DescriptiveImageFileId { get; set; }

        DateTime End { get; set; }

        string Goal { get; set; }

        string Name { get; set; }

        string Proposal { get; set; }

        DateTime Start { get; set; }

        ICollection<string> Tags { get; set; }

        int Target { get; set; }
    }
}