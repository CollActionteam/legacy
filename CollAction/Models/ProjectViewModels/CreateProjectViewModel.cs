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

        [Required(ErrorMessage = "Geef je project een unieke naam.")]
        [Display(Name = "Naam project")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Welke actie stel je voor te ondernemen met hoeveel mensen?")]
        [Display(Name = "Voorstel", Prompt = "Bv. \"Als X mensen toezeggen om Y te doen, dan doen we het met zijn allen!\"")]
        [MaxLength(300)]
        public string Proposal { get; set; }

        [Required(ErrorMessage = "Geef een korte beschrijving van de actie die je voorstelt te ondernemen en het probleem dat je probeert op te lossen.")]
        [Display(Name = "Korte beschrijving", Prompt = "Bv. \"In januari doen we allemaal XYZ, omdat…\"")]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Beschrijf wat de impact is van je actie - waarom is deze actie nemen belangrijk?")]
        [Display(Name = "Doel/Impact", Prompt = "Max 1000 tekens")]
        [MaxLength(1000)]
        public string Goal { get; set; }

        [Display(Name = "Andere opmerkingen", Prompt = "Bv. meer achtergrond, hoe gaat het project in zijn werk, FAQs, over de initatiefnemer.")]
        [MaxLength(2000)]
        public string CreatorComments { get; set; }

        [Required]
        [Display(Name = "Categorie")]
        public int CategoryId { get; set; }

        [Display(Name = "Locatie")]
        public int? LocationId { get; set; }

        [Required(ErrorMessage = "Wat is het target? Met andere woorden: hoeveel mensen moeten er minimaal meedoen om de actie door te laten gaan?")]
        [DataType(DataType.Text)] // Force this to text rather than .Int to prevent browsers displaying spin buttons (up down arrows) by default.
        [Range(1, 1000000, ErrorMessage = "Er kunnen maximaal een miljoen mensen meedoen aan een project. Contacteer ons team als je een hoger aantal wilt invoeren.")]
        public int Target { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Startdatum")]
        [WithinMonthsAfterToday(12, ErrorMessage = "Projecten moeten binnen 12 maanden starten.")]
        public DateTime Start { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Deadline")]
        [WithinMonthsAfterDateProperty(12, "Start", ErrorMessage = "De deadline is maximaal 12 maanden na de startdatum.")]
        public DateTime End { get; set; }

        [Display(Name = "Beschrijving banner afbeelding")]
        [MaxLength(50)]
        public string BannerImageDescription { get; set; }

        [Display(Name = "Banner afbeelding", Prompt = "1366x432 JPEG, GIF, PNG, BMP")]
        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        [MaxImageDimensions(1366, 432)]
        public IFormFile BannerImageUpload { get; set; }

        [Display(Name = "Beschrijving extra afbeelding")]
        [MaxLength(50)]
        public string DescriptiveImageDescription { get; set; }

        [Display(Name = "Extra afbeelding", Prompt = "777x370 JPEG, GIF, PNG, BMP")]
        [FileSize(1024000)] // 1MB
        [FileType("jpg", "jpeg", "gif", "png", "bmp")]
        [MaxImageDimensions(777, 370)]
        public IFormFile DescriptiveImageUpload { get; set; }

        [Display(Name = "YouTube Video Link", Prompt = "YouTube link naar een relevant filmpje. Bv. \"https://www.youtube.com/watch?v=xnIJo91Gero\"")]
        [YouTubeLink]
        public string DescriptionVideoLink { get; set; }

        [Display(Name = "Hashtag", Prompt = "Max 30 tekens. Graag zonder #-teken invoeren. Bv. 'tag1;tag2'.")]
        [MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$", ErrorMessage = "Graag geen spaties of #-tekens gebruiken. Moet minimaal 1 letter bevatten. Meerdere tags scheiden dmv een \";\"")]
        public string Hashtag { get; set; }
    }
}