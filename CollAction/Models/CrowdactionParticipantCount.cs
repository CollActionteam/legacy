using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    /// <summary>
    /// This is a materialized view of the aggregate Participants.Sum(participant => participant.User.RepresentsNumberParticipants) + crowdaction.AnonymousUserParticipants
    /// </summary>
    public sealed class CrowdactionParticipantCount
    {
        public CrowdactionParticipantCount(int crowdactionId, int count)
        {
            CrowdactionId = crowdactionId;
            Count = count;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CrowdactionId { get; set; }

        [ForeignKey("CrowdactionId")]
        public Crowdaction? Crowdaction { get; set; }

        public int Count { get; set; }
    }
}
