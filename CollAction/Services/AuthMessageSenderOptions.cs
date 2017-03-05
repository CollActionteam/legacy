namespace CollAction.Services
{
    public class AuthMessageSenderOptions
    {
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string SendGridKey { get; set; }
    }
}