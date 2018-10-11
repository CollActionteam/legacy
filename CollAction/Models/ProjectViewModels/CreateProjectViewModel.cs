using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CollAction.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using CollAction.ValidationAttributes;

namespace CollAction.Models
{
    public class CreateProjectViewModel
    {
        public SelectList Categories { get; set; }
        public SelectList Locations { get; set; }

        public CreateProjectViewModel() 
        {
        }

        [Required(ErrorMessage = "You must provide a unique name for your project")]
        [StringLength(50, ErrorMessage = "Keep the name short, no more then 50 characters")]
        [Display(Name = "Project name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please specify the target number of collaborators required for your project to be deemed a success")]
        [DataType(DataType.Text)] // Force this to text rather than .Int to prevent browsers displaying spin buttons (up down arrows) by default.
        [Range(1, 1000000, ErrorMessage = "You can choose up to a maximum of one million participants as your target number")]
        [Display(Prompt = "Target")]
        public int Target { get; set; }

        [Required(ErrorMessage = "Describe your proposal.")]
        [StringLength(300, ErrorMessage = "You best keep the proposal short, no more then 300 characters")]
        [Display(Prompt = "e.g. \"If X people commit to doing Y, we'll all do it together!\"")]
        public string Proposal { get; set; }

        public string ProjectStarterFirstName { get; set; }

        public string ProjectStarterLastName { get; set; }        

        [RichTextRequired(ErrorMessage = "Give a succinct description of the issues your project is designed to address")]
        [StringLength(10000, ErrorMessage = "Please use no more then 10.000 characters")]
        [SecureRichText]
        [Display(Name = "Short description", Prompt = "E.g Reduce plastic waste and save our oceans!")]
        public string Description { get; set; }

        [RichTextRequired(ErrorMessage = "Describe what you hope to have achieved upon successful completion of your project")]
        [StringLength(10000, ErrorMessage = "Please use no more then 10.000 characters")]
        [SecureRichText]
        [Display(Name = "Goal/Impact", Prompt = "Max 1000 characters")]
        public string Goal { get; set; }

        [StringLength(20000, ErrorMessage = "Please use no more then 20.000 characters")]
        [SecureRichText]
        [Display(Name = "Other comments", Prompt = "e.g. Background, process, FAQs, about the initiator")]
        public string CreatorComments { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [WithinMonthsAfterToday(12, ErrorMessage = "Please ensure your project starts within the next 12 months")]
        [Display(Name = "Start date")]
        public DateTime Start { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [WithinMonthsAfterDateProperty(12, "Start", ErrorMessage = "The deadline must be within a year of the start date")]
        [Display(Name = "Deadline")]
        public DateTime End { get; set; }

        [MaxLength(50, ErrorMessage = "Keep your description short, no more then 50 characters")]
        [Display(Name = "Banner image description")]
        public string BannerImageDescription { get; set; }

        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        [Display(Name = "Banner image")]
        public IFormFile BannerImageUpload { get; set; }

        [MaxLength(50, ErrorMessage = "Keep your description short, no more then 50 characters")]
        [Display(Name = "Image description")]
        public string DescriptiveImageDescription { get; set; }

        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        [Display(Name = "Description image")]
        public IFormFile DescriptiveImageUpload { get; set; }

        [RegularExpression(@"^(http|https)://www.youtube.com/watch\?v=((?:\w|-){11}?)$", ErrorMessage="Only YouTube links of the form http://www.youtube.com/watch?v=<your-11-character-video-id> are accepted.")]
        [Display(Name = "YouTube Video Link", Prompt = "Descriptive Video. e.g. http://www.youtube.com/watch?v=-wtIMTCHWuI")]
        public string DescriptionVideoLink { get; set; }

        [MaxLength(30, ErrorMessage = "Please keep the number of hashtags civil, no more then 30 characters")]
        [RegularExpression(@"^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$", ErrorMessage = "No spaces or #, must contain a letter, can contain digits and underscores. Seperate multiple tags with a colon ';'")]
        [Display(Name = "Hashtag", Prompt = "No #, seperate with ; E.g. 'tag1;tag2'")]
        public string Hashtag { get; set; }
    }
}