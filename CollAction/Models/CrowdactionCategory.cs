namespace CollAction.Models
{
    public sealed class CrowdactionCategory
    {
        public CrowdactionCategory(Category category) : this(0, category)
        {
        }

        public CrowdactionCategory(int crowdactionId, Category category)
        {
            CrowdactionId = crowdactionId;
            Category = category;
        }

        public int CrowdactionId { get; set; }

        public Category Category { get; set; }

        public Crowdaction? Crowdaction { get; set; }
    }
}
