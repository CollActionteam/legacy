namespace CollAction.Services.User.Models
{
    public class UpdatedUser
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsSubscribedNewsletter { get; set; }
    }
}
