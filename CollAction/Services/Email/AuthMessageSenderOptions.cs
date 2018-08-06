using Amazon;

namespace CollAction.Services.Email
{
    public class AuthMessageSenderOptions
    {
        public string FromAddress { get; set; }
        public string SesAwsAccessKeyID { get; set; }
        public string SesAwsAccessKey { get; set; }
        public string SesRegion { get; set; }
    }
}