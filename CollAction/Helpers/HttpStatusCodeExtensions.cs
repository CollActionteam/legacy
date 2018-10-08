using System.Net;

namespace CollAction.Helpers
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccess(this HttpStatusCode code)
            => (int)code >= 200 && (int)code <= 299;
    }
}
