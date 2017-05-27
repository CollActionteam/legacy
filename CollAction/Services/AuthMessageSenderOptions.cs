using Amazon;

namespace CollAction.Services
{
    public class AuthMessageSenderOptions
    {
        public string FromAddress { get; set; }
        public string SesAwsAccessKeyID { get; set; }
        public string SesAwsAccessKey { get; set; }
        public RegionEndpoint Region { get; set; }
    }
}