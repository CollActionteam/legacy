using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.User.Models
{
    public class UpdatedUser
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public int RepresentsNumberUsers { get; set; }

        public bool IsSubscribedNewsletter { get; set; }
    }
}
