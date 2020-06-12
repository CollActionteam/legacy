using CollAction.Models;
using CollAction.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Crowdactions.Models
{
    public sealed class UpdatedCrowdaction
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a unique name for your crowdaction")]
        [StringLength(50, ErrorMessage = "Keep the name short, no more then 50 characters")]
        public string Name { get; set; } = null!;

        [Range(1, 1000000, ErrorMessage = "You can choose up to a maximum of one million participants as your target number")]
        public int Target { get; set; }

        [Required(ErrorMessage = "Describe your proposal.")]
        [StringLength(300, ErrorMessage = "You best keep the proposal short, no more then 300 characters")]
        public string Proposal { get; set; } = null!;

        [Required(ErrorMessage = "Give a succinct description of the issues your crowdaction is designed to address")]
        [StringLength(10000, ErrorMessage = "Please use no more then 10.000 characters")]
        [SecureHtml]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Describe what you hope to have achieved upon successful completion of your crowdaction")]
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

        [MinLength(1)]
        [MaxLength(30)]
        public string? InstagramName { get; set; }

        public int? CardImageFileId { get; set; }

        public int? BannerImageFileId { get; set; }

        public int? DescriptiveImageFileId { get; set; }

        [RegularExpression(@"^https://www.youtube(\-nocookie)?.com/embed/[A-Za-z_\-0-9]+$", ErrorMessage = "Only embedded youtube links are accepted. If you don't know how to get one, see: https://support.google.com/youtube/answer/171780?hl=en")]
        public string? DescriptionVideoLink { get; set; }

        public CrowdactionDisplayPriority DisplayPriority { get; set; }

        public CrowdactionStatus Status { get; set; }

        [Range(0, int.MaxValue)]
        public int NumberCrowdactionEmailsSent { get; set; }

        [Tags]
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        [Categories]
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
