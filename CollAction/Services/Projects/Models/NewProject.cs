using CollAction.Models;
using System;
using System.Collections.Generic;

namespace CollAction.Services.Projects.Models
{
    public class NewProject : IProjectModel
    {
        public string Name { get; set; }

        public int Target { get; set; }

        public string Proposal { get; set; }

        public string Description { get; set; }

        public string Goal { get; set; }

        public string CreatorComments { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int? BannerImageFileId { get; set; }

        public int? DescriptiveImageFileId { get; set; }

        public string DescriptionVideoLink { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ICollection<string> Tags { get; set; }
    }
}
