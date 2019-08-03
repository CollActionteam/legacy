using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using CollAction.Services.Sitemap;
using System.Threading;

namespace CollAction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISitemapService sitemapService;
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger, ISitemapService sitemapService)
        {
            this.sitemapService = sitemapService;
            this.logger = logger;
        }

        public IActionResult Index()
            => View();

        [Route("robots.txt")]
        public ContentResult Robots()
            => Content(sitemapService.RobotsTxt, "text/plain", Encoding.UTF8);

        [Route("sitemap.xml")]
        public async Task<ContentResult> Sitemap(CancellationToken cancellationToken)
            => Content((await sitemapService.GetSitemap(cancellationToken)).ToString(), "text/xml", Encoding.UTF8);

        [Route("error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionHandlerPathFeature.Error, "An error has occurred at: {0}", exceptionHandlerPathFeature.Path);
            return View();
        }
    }
}
