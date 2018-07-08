using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.AdminViewModels
{
    public class ManageUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Represents number of actual participants")]
        [Range(1, double.MaxValue)]
        public int RepresentsNumberParticipants { get; set; }
    }
}
