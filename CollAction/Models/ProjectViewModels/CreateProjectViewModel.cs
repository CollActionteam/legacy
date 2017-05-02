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
        public SelectList Categories;
        public SelectList Locations;

        public CreateProjectViewModel() { }

        public CreateProjectViewModel(SelectList categories, SelectList locations)
        {
            this.Categories = categories;
            this.Locations = locations; 
        }

        [Required(ErrorMessage = "You must provide a unique name for your project.")]
        [Display(Name = "Project name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Describe your proposal.")]
        [Display(Prompt = "e.g. \"If X people commit to doing Y, we'll all do it together!\"")]
        [MaxLength(300)]
        public string Proposal { get; set; }

        [Required(ErrorMessage = "Give a succinct description of the issues your project is designed to address")]
        [Display(Name = "Short description", Prompt = "E.g Reduce plastic waste and save our oceans!")]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Describe what you hope to have achieved on successful completion of your project")]
        [Display(Name = "Goal/Impact", Prompt = "Max 1000 characters")]
        [MaxLength(1000)]
        public string Goal { get; set; }

        [Display(Name = "Other comments", Prompt = "e.g. Background, process, FAQs, about the initiator")]
        [MaxLength(2000)]
        public string CreatorComments { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Required(ErrorMessage = "Please specify the target number of collaborators required for your project to be deemed a success")]
        [Range(1, Double.MaxValue, ErrorMessage = "Please enter the target number of participants.")]
        public int Target { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime Start { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Deadline")]
        public DateTime End { get; set; }

        [Display(Name = "Banner image", Prompt = "1024x768px JPEG, GIF, PNG, BMP")]
        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        [MaxImageDimensions(1024, 768)]
        public IFormFile BannerImageUpload { get; set; }

        public bool HasBannerImageUpload { get { return BannerImageUpload != null && BannerImageUpload.Length > 0; } }

        [Display(Name = "YouTube Video Link", Prompt = "Descriptive Video. e.g. http://www.youtube.com/watch?v=-wtIMTCHWuI")]
        [YouTubeLink]
        public string DescriptionVideoLink { get; set; }

        [Display(Name = "Hashtag", Prompt = "Max 30 characters. e.g. 'tag1;tag2'")]
        [MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$", ErrorMessage = "No spaces, must contain a letter, can contain digits and underscores. Seperate multiple tags with a colon ';'.")]
        public string Hashtag { get; set; }
    }
}