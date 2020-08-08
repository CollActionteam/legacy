using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Proxy
{
    /*
     * We're using this as a simple (probably incomplete) privacy proxy, 
     * to stop people from being directly seen/fingerprinted/tracked by facebook 
     * when visiting our site when we need to display stuff from instagram/facebook
     */
    public sealed class ProxyService : IProxyService
    {
        private readonly HttpClient proxyClient;
        /*
         * As a complete list as I can get of facebook/instagram cdns
         * We don't want to be an open proxy (for security/liability), 
         * so lets restrict what hosts are allowed
         */
        private static readonly string[] AllowedHosts =
            new string[]
            {
                "fbcdn.net",
                "cdninstagram.com",
                "fbsbx.com",
                "tfbnw.net",
                "fb.me",
                "facebook.com.edgesuite.net",
                "facebook.com.edgekey.net",
                "facebook.net.edgekey.net",
                "facebook-web-clients.appspot.com",
                "fbcdn-profile-a.akamaihd.net",
                "fbsbx.com.online-metrix.net",
                "instagramstatic-a.akamaihd.net",
                "akamaihd.net.edgesuite.net",
                "internet.org"
            };

        public ProxyService(HttpClient proxyClient)
        {
            this.proxyClient = proxyClient;
        }

        public async Task<IActionResult> Proxy(Uri url, CancellationToken token)
        {
            if (CanProxy(url))
            {
                var result = await proxyClient.GetAsync(url, token).ConfigureAwait(false);
                result.EnsureSuccessStatusCode();
                var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return new FileStreamResult(stream, new MediaTypeHeaderValue(result.Content.Headers.ContentType.MediaType))
                {
                    LastModified = result.Content.Headers.LastModified,
                    EnableRangeProcessing = true
                };
            }
            else
            {
                return new StatusCodeResult(405);
            }
        }

        private static bool CanProxy(Uri url)
            => AllowedHosts.Any(h => url.Host.EndsWith(h, StringComparison.Ordinal));
    }
}
