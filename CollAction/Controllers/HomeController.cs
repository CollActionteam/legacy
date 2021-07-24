using CollAction.Services.Sitemap;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CollAction.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly ISitemapService sitemapService;
        private readonly ILogger<HomeController> logger;

        public HomeController(ISitemapService sitemapService, ILogger<HomeController> logger)
        {
            this.sitemapService = sitemapService;
            this.logger = logger;
        }

        [Route("sitemap")]
        public async Task<IActionResult> Sitemap(CancellationToken token)
        {
            XDocument sitemap = await sitemapService.GetSitemap(token).ConfigureAwait(false);
            return new ContentResult()
            {
                Content = $"{sitemap.Declaration}{sitemap.ToString(SaveOptions.DisableFormatting)}",
                ContentType = "text/xml",
                StatusCode = 200
            };
        }

        [Route("error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionHandlerPathFeature.Error, "An error has occurred at: {0}", exceptionHandlerPathFeature.Path);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Json(new { error = "An internal error has occured" });
        }
    }
}
