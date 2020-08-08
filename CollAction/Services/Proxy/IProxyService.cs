using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Proxy
{
    public interface IProxyService
    {
        public Task<IActionResult> Proxy(Uri url, CancellationToken token);
    }
}
