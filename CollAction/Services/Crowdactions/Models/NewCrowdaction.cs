using CollAction.Models;
using CollAction.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.Services.Crowdactions.Models
{
    public sealed class NewCrowdaction
    {
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

        [DataType(DataType.Date)]
        [WithinMonthsAfterToday(12, ErrorMessage = "Please ensure your crowdaction starts within the next 12 months")]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [WithinMonthsAfterDateProperty(12, "Start", ErrorMessage = "The deadline must be within a year of the start date")]
        public DateTime End { get; set; }

        [MinLength(1)]
        [MaxLength(30)]
        public string? InstagramUser { get; set; }

        public int? CardImageFileId { get; set; }

        public int? BannerImageFileId { get; set; }

        public int? DescriptiveImageFileId { get; set; }

        [RegularExpression(@"^https://www.youtube(\-nocookie)?.com/embed/[A-Za-z_\-0-9]+$", ErrorMessage = "Only embedded youtube links are accepted. If you don't know how to get one, see: https://support.google.com/youtube/answer/171780?hl=en")]
        public string? DescriptionVideoLink { get; set; }

        [Categories]
        public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();

        [Tags]
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}
