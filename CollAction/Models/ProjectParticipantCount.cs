using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    /// <summary>
    /// This is a materialized view of the aggregate Participants.Sum(participant => participant.User.RepresentsNumberParticipants) + project.AnonymousUserParticipants
    /// </summary>
    public sealed class ProjectParticipantCount
    {
        public ProjectParticipantCount(int projectId, int count)
        {
            ProjectId = projectId;
            Count = count;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProjectId { get; set; }

        private Project? project;
        [ForeignKey("ProjectId")]
        public Project Project
        {
            get => project ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Project)}");
            set => project = value;
        }

        public int Count { get; set; }
    }
}
