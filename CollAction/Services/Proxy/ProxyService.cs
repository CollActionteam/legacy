using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Proxy
{
    public sealed class ProxyService : IProxyService
    {
        private readonly HttpClient proxyClient;

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
            => url.Host.EndsWith("fbcdn.net", StringComparison.Ordinal);
    }
}
