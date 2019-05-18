using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class CommitViewModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Please enter an e-mail address")]
        [EmailAddress(ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }

    }
}