namespace CollAction.Models
{
    public class ThankYouCommitViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectNameUriPart { get; set; }

        public string ProjectProposal { get; set; }

        public bool IsActive { get; set; }

        public string ProjectLink => $"/Projects/{ProjectNameUriPart}/{ProjectId}/Details";
    }
}
