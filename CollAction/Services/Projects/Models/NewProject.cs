using CollAction.Models;
using CollAction.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Projects.Models
{
    public sealed class NewProject
    {
        [Required(ErrorMessage = "You must provide a unique name for your project")]
        [StringLength(50, ErrorMessage = "Keep the name short, no more then 50 characters")]
        public string Name { get; set; }

        [Range(1, 1000000, ErrorMessage = "You can choose up to a maximum of one million participants as your target number")]
        public int Target { get; set; }

        [Required(ErrorMessage = "Describe your proposal.")]
        [StringLength(300, ErrorMessage = "You best keep the proposal short, no more then 300 characters")]
        public string Proposal { get; set; }

        [Required(ErrorMessage = "Give a succinct description of the issues your project is designed to address")]
        [StringLength(10000, ErrorMessage = "Please use no more then 10.000 characters")]
        [SecureHtml]
        public string Description { get; set; }

        [Required(ErrorMessage = "Describe what you hope to have achieved upon successful completion of your project")]
        [StringLength(10000, ErrorMessage = "Please use no more then 10.000 characters")]
        [SecureHtml]
        public string Goal { get; set; }

        [StringLength(20000, ErrorMessage = "Please use no more then 20.000 characters")]
        [SecureHtml]
        public string CreatorComments { get; set; }

        [DataType(DataType.Date)]
        [WithinMonthsAfterToday(12, ErrorMessage = "Please ensure your project starts within the next 12 months")]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [WithinMonthsAfterDateProperty(12, "Start", ErrorMessage = "The deadline must be within a year of the start date")]
        public DateTime End { get; set; }

        public int? BannerImageFileId { get; set; }

        public int? DescriptiveImageFileId { get; set; }

        [RegularExpression(@"^https://www.youtube.com/watch\?v=[^& ]+$", ErrorMessage = "Only YouTube links of the form http://www.youtube.com/watch?v=<your-video-id> are accepted.")]
        public string DescriptionVideoLink { get; set; }

        [Categories]
        public ICollection<Category> Categories { get; set; }

        [Tags]
        public ICollection<string> Tags { get; set; }
    }
}
