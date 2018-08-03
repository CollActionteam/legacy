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

        [Required(ErrorMessage = "You must provide a unique name for your project.")]
        [Display(Name = "Project name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please specify the target number of collaborators required for your project to be deemed a success")]
        [DataType(DataType.Text)] // Force this to text rather than .Int to prevent browsers displaying spin buttons (up down arrows) by default.
        [Range(1, 1000000, ErrorMessage = "You can choose up to a maximum of one million participants as your target number.")]
        public int Target { get; set; }

        [Required(ErrorMessage = "Describe your proposal.")]
        [Display(Prompt = "e.g. \"If X people commit to doing Y, we'll all do it together!\"")]
        [MaxLength(300)]
        public string Proposal { get; set; }

        public string ProjectStarterFirstName { get; set; }

        public string ProjectStarterLastName { get; set; }        

        [Required(ErrorMessage = "Give a succinct description of the issues your project is designed to address")]
        [Display(Name = "Short description", Prompt = "E.g Reduce plastic waste and save our oceans!")]
        [MaxLength(10000)]
        [SecureRichText]
        public string Description { get; set; }

        [Required(ErrorMessage = "Describe what you hope to have achieved upon successful completion of your project")]
        [Display(Name = "Goal/Impact", Prompt = "Max 1000 characters")]
        [MaxLength(10000)]
        [SecureRichText]
        public string Goal { get; set; }

        [Display(Name = "Other comments", Prompt = "e.g. Background, process, FAQs, about the initiator")]
        [MaxLength(20000)]
        [SecureRichText]
        public string CreatorComments { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        [WithinMonthsAfterToday(12, ErrorMessage = "Please ensure your project starts within the next 12 months.")]
        public DateTime Start { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Deadline")]
        [WithinMonthsAfterDateProperty(12, "Start", ErrorMessage = "The deadline must be within a year of the start date.")]
        public DateTime End { get; set; }

        [Display(Name = "Banner image description")]
        [MaxLength(50)]
        public string BannerImageDescription { get; set; }

        [Display(Name = "Banner image", Prompt = "2732x864 JPEG, GIF, PNG, BMP")]
        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        public IFormFile BannerImageUpload { get; set; }

        [Display(Name = "Image description")]
        [MaxLength(50)]
        public string DescriptiveImageDescription { get; set; }

        [Display(Name = "Description image", Prompt = "1088x518 JPEG, GIF, PNG, BMP")]
        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        public IFormFile DescriptiveImageUpload { get; set; }

        [Display(Name = "YouTube Video Link", Prompt = "Descriptive Video. e.g. http://www.youtube.com/watch?v=-wtIMTCHWuI")]
        [YouTubeLink]
        public string DescriptionVideoLink { get; set; }

        [Display(Name = "Hashtag", Prompt = "No #, seperate with ; E.g. 'tag1;tag2'")]
        [MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$", ErrorMessage = "No spaces or #, must contain a letter, can contain digits and underscores. Seperate multiple tags with a colon ';'.")]
        public string Hashtag { get; set; }
    }
}