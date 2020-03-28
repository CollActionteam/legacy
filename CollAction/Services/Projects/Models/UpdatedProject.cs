using CollAction.Models;
using CollAction.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Projects.Models
{
    public sealed class UpdatedProject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a unique name for your project")]
        [StringLength(50, ErrorMessage = "Keep the name short, no more then 50 characters")]
        public string Name { get; set; } = null!;

        [Range(1, 1000000, ErrorMessage = "You can choose up to a maximum of one million participants as your target number")]
        public int Target { get; set; }

        [Required(ErrorMessage = "Describe your proposal.")]
        [StringLength(300, ErrorMessage = "You best keep the proposal short, no more then 300 characters")]
        public string Proposal { get; set; } = null!;

        [Required(ErrorMessage = "Give a succinct description of the issues your project is designed to address")]
        [StringLength(10000, ErrorMessage = "Please use no more then 10.000 characters")]
        [SecureHtml]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Describe what you hope to have achieved upon successful completion of your project")]
        [StringLength(10000, ErrorMessage = "Please use no more then 10.000 characters")]
        [SecureHtml]
        public string Goal { get; set; } = null!;

        [StringLength(20000, ErrorMessage = "Please use no more then 20.000 characters")]
        [SecureHtml]
        public string? CreatorComments { get; set; }

        public string? OwnerId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        public DateTime End { get; set; }

        public int? BannerImageFileId { get; set; }

        public int? DescriptiveImageFileId { get; set; }

        [RegularExpression(@"^https://www.youtube(\-nocookie)?.com/embed/[A-Za-z_\-0-9]+$", ErrorMessage = "Only embedded youtube links are accepted. If you don't know how to get one, see: https://support.google.com/youtube/answer/171780?hl=en")]
        public string? DescriptionVideoLink { get; set; }

        public ProjectDisplayPriority DisplayPriority { get; set; }

        public ProjectStatus Status { get; set; }

        [Range(0, int.MaxValue)]
        public int NumberProjectEmailsSend { get; set; }

        [Tags]
        public ICollection<string> Tags { get; set; } = new List<string>();

        [Categories]
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
