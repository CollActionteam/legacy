using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CollAction.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required(ErrorMessage = "You must provide a name for your project.")]
        [Display(Name = "Project name")]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Give a succinct description of the issues your project is designed to address")]
        [Display(Prompt = "E.g Reduce plastic waste and save our oceans!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Describe what you hope to have achieved on successful completion of your project")]
        [Display(Prompt = "Max 1000 characters")]
        [MaxLength(1024)]
        public string Goal { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Required(ErrorMessage = "Please specify the target number of collaborators required for your project to be deemed a success")]
        [Range(1, Double.MaxValue, ErrorMessage = "Please enter the target number of participants.")]
        public int Target { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
    }
}