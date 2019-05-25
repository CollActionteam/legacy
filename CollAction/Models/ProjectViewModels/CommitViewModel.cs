using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class CommitViewModel
    {
        [Required]
        public int ProjectId { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }

    }
}