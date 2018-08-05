using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.ProjectViewModels
{
    public sealed class ChangeSubscriptionFromTokenViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UnsubscribeToken { get; set; }
    }
}
