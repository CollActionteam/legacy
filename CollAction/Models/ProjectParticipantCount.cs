using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    /// <summary>
    /// This is a materialized view of the aggregate Participants.Sum(participant => participant.User.RepresentsNumberParticipants) + project.AnonymousUserParticipants
    /// </summary>
    public class ProjectParticipantCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public int Count { get; set; }
    }
}
