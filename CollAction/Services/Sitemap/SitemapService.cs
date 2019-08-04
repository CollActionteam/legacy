using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Image;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CollAction.Services.Sitemap
{
    public class SitemapService : ISitemapService
    {
        private static XNamespace urlsetNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static XNamespace imageNamespace = "http://www.google.com/schemas/sitemap-image/1.1";
        private readonly ApplicationDbContext context;
        private readonly IImageService imageService;
        private readonly SiteOptions siteOptions;

        public SitemapService(ApplicationDbContext context, IImageService imageService, IOptions<SiteOptions> siteOptions)
        {
            this.context = context;
            this.imageService = imageService;
            this.siteOptions = siteOptions.Value;
        }

        public string RobotsTxt
            => $@"User-agent: *
Sitemap: https://{siteOptions.PublicAddress}/sitemap.xml
Disallow: /Admin/
Disallow: /Account/
Disallow: /Manage/";

        public async Task<XDocument> GetSitemap(CancellationToken cancellationToken)
        {
            object[] homepageUrls = new[]
            {
                new XElement(urlsetNamespace + "url", new XElement(urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "/Home")),
                new XElement(urlsetNamespace + "url", new XElement(urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "/About")),
                new XElement(urlsetNamespace + "url", new XElement(urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "/Find")),
                new XElement(urlsetNamespace + "url", new XElement(urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "/CrowdActingFestival"))
            };

            object[] projectUrls = await context.Projects
                .Where(p => p.Status != ProjectStatus.Deleted && p.Status != ProjectStatus.Hidden)
                .Include(p => p.BannerImage)
                .Select(project => GetSitemapProjectEntry(project))
                .ToArrayAsync(cancellationToken);

            object[] rootContent = homepageUrls
                .Concat(projectUrls)
                .Concat(new[]
                {
                    new XAttribute(XNamespace.Xmlns + "image", imageNamespace)
                }).ToArray();

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(urlsetNamespace + "urlset", rootContent));
        }

        private XElement GetSitemapProjectEntry(Models.Project project)
        {
            List<XElement> projectElements = new List<XElement>(3)
            {
                new XElement(urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + project.Url)
            };
            if (project.BannerImageFileId != null)
            {
                projectElements.Add(
                    new XElement(
                        imageNamespace + "image", 
                        new[]
                        {
                            new XElement(imageNamespace + "loc", "https://" + siteOptions.PublicAddress + imageService.GetUrl(project.BannerImage)),
                            new XElement(imageNamespace + "caption", project.BannerImage.Description)
                        }));
            }

            return new XElement(urlsetNamespace + "url", projectElements.ToArray());
        }
    }
}
