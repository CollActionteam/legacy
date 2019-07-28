using Microsoft.AspNetCore.Mvc;
using System.Text;
using CollAction.Data;
using System.Threading.Tasks;
using CollAction.Services.Image;
using CollAction.Services.Project;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using CollAction.Services.Sitemap;

namespace CollAction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectService _projectService;
        private readonly IImageService _imageService;
        private readonly ISitemapService _sitemapService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, IProjectService projectService, IImageService imageService, ILogger<HomeController> logger, ISitemapService sitemapService)
        {
            _context = context;
            _projectService = projectService;
            _imageService = imageService;
            _sitemapService = sitemapService;
            _logger = logger;
        }

        public IActionResult Index()
            => View();

        public IActionResult Privacy()
            => View();

        public IActionResult About()
            => View();

        public IActionResult CrowdActingFestival()
            => View();

        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError(exceptionHandlerPathFeature.Error, "An error has occurred at: {0}", exceptionHandlerPathFeature.Path);
            return View();
        }

        public ContentResult Robots()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.Append("Sitemap: https://");
            sb.Append(Url.ActionContext.HttpContext.Request.Host);
            sb.AppendLine(Url.Action("Sitemap"));
            sb.AppendLine("Disallow: /Admin/");
            sb.AppendLine("Disallow: /Account/");
            sb.AppendLine("Disallow: /Manage/");
            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }

        public async Task<ContentResult> Sitemap()
            => Content((await _sitemapService.GetSitemap()).ToString(), "text/xml", Encoding.UTF8);
    }
}
