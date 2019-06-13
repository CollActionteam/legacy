using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Text;
using CollAction.Data;
using Microsoft.AspNetCore.Hosting;
using CollAction.Helpers;
using System.Threading.Tasks;
using CollAction.Services.Image;
using CollAction.Services.Project;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace CollAction.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly ApplicationDbContext _context;
        private readonly IProjectService _projectService;
        private readonly IImageService _imageService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IStringLocalizer<HomeController> localizer, ApplicationDbContext context, IProjectService projectService, IImageService imageService, ILogger<HomeController> logger)
        {
            _localizer = localizer;
            _context = context;
            _projectService = projectService;
            _imageService = imageService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = _localizer["About CollAction"];
            return View();
        }

        public IActionResult CrowdActingFestival()
        {
            ViewData["Title"] = _localizer["Make The World Great Again Festival"];
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public ViewResult FAQ()
        {
            ViewData["Title"] = _localizer["Frequently Asked Questions"];
            return View();
        }

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
            => Content((await new SitemapHelper(_context, Url, _projectService, _imageService).GetSitemap()).ToString(), "text/xml", Encoding.UTF8);
    }
}
