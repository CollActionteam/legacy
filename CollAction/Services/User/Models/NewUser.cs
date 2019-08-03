namespace CollAction.Services.User.Models
{
    public class NewUser
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public bool IsSubscribedNewsletter { get; set; }
    }
}
