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
    public sealed class ProxyService : IProxyService
    {
        private readonly HttpClient proxyClient;
        private static string[] AllowedHosts =
            new string[]
            {
                "fbcdn.net",
                "cdninstagram.com"
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
                    LastModified = result.Content.Headers.LastModified
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
