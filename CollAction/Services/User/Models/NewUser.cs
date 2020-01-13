using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.User.Models
{
    public class NewUser
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsSubscribedNewsletter { get; set; }
    }
}
