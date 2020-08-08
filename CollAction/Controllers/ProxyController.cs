using CollAction.Services.Proxy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("proxy")]
    [ApiController]
    public sealed class ProxyController
    {
        private readonly IProxyService proxyService;

        public ProxyController(IProxyService proxyService)
        {
            this.proxyService = proxyService;
        }

        [HttpGet]
        public Task<IActionResult> Proxy(Uri url, CancellationToken token)
            => proxyService.Proxy(url, token);
    }
}
