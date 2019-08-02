using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CollAction.Services.Sitemap
{
    public class SitemapService : ISitemapService
    {
        private static XNamespace _urlsetNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static XNamespace _imageNamespace = "http://www.google.com/schemas/sitemap-image/1.1";
        private readonly ApplicationDbContext context;
        private readonly IImageService imageService;
        private readonly SiteOptions siteOptions;

        public SitemapService(ApplicationDbContext context, IImageService imageService, IOptions<SiteOptions> siteOptions)
        {
            this.context = context;
            this.imageService = imageService;
            this.siteOptions = siteOptions.Value;
        }

        public async Task<XDocument> GetSitemap()
        {
            object[] homepageUrls = new[]
            {
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "#/Home")),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "#/About")),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "#/Find")),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + "#/CrowdActingFestival"))
            };

            object[] projectUrls = await context.Projects
                .Where(p => p.Status != ProjectStatus.Deleted && p.Status != ProjectStatus.Hidden)
                .Include(p => p.BannerImage)
                .Select(project => GetSitemapProjectEntry(project))
                .ToArrayAsync();

            object[] rootContent = homepageUrls
                .Concat(projectUrls)
                .Concat(new[]
                {
                    new XAttribute(XNamespace.Xmlns + "image", _imageNamespace)
                }).ToArray();

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(_urlsetNamespace + "urlset", rootContent));
        }

        private XElement GetSitemapProjectEntry(Models.Project project)
        {
            List<XElement> projectElements = new List<XElement>(3)
            {
                new XElement(_urlsetNamespace + "loc", "https://" + siteOptions.PublicAddress + project.Url)
            };
            if (project.BannerImageFileId != null)
            {
                projectElements.Add(new XElement(_imageNamespace + "image", new[]
                {
                    new XElement(_imageNamespace + "loc", "https://" + siteOptions.PublicAddress + imageService.GetUrl(project.BannerImage)),
                    new XElement(_imageNamespace + "caption", project.BannerImage.Description)
                }));
            }
            return new XElement(_urlsetNamespace + "url", projectElements.ToArray());
        }
    }
}
