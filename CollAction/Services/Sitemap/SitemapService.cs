using CollAction.GraphQl.Mutations.Input;
using CollAction.Models;
using CollAction.Services.Crowdactions;
using CollAction.Services.Image;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CollAction.Services.Sitemap
{
    public sealed class SitemapService : ISitemapService
    {
        private readonly static XNamespace SitemapSchema = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly static XNamespace SitemapImageSchema = "http://www.google.com/schemas/sitemap-image/1.1";
        private readonly static XNamespace SitemapVideoSchema = "http://www.google.com/schemas/sitemap-video/1.1";
        private readonly static XAttribute[] ExtraNamespaces = new[]
        {
            new XAttribute(XNamespace.Xmlns + "video", SitemapVideoSchema),
            new XAttribute(XNamespace.Xmlns + "image", SitemapImageSchema)
        };
        private readonly ILogger<SitemapService> logger;
        private readonly ICrowdactionService crowdactionService;
        private readonly IImageService imageService;
        private readonly SiteOptions siteOptions;

        public SitemapService(ICrowdactionService crowdactionService, IImageService imageService, IOptions<SiteOptions> siteOptions, ILogger<SitemapService> logger)
        {
            this.logger = logger;
            this.crowdactionService = crowdactionService;
            this.imageService = imageService;
            this.siteOptions = siteOptions.Value;
        }

        public async Task<XDocument> GetSitemap(CancellationToken cancellationToken)
        {
            XElement[] normalSitemapEntries =
                new[]
                {
                    GetUrlSitemapEntry(new Uri("/", UriKind.Relative), "always", 1.0m),
                    GetUrlSitemapEntry(new Uri("/privacy-policy", UriKind.Relative), "monthly", 0.1m),
                    GetUrlSitemapEntry(new Uri("/about", UriKind.Relative), "monthly", 0.3m),
                    GetUrlSitemapEntry(new Uri("/donate", UriKind.Relative), "monthly", 0.3m),
                    GetUrlSitemapEntry(new Uri("/find", UriKind.Relative), "always", 0.3m)
                };

            List<Crowdaction> crowdactions = 
                await crowdactionService
                    .SearchCrowdactions(null, SearchCrowdactionStatus.Active)
                    .Include(c => c.BannerImage)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

            IEnumerable<XElement> crowdactionEntries = crowdactions.Select(GetCrowdactionSitemapEntry);

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(
                    SitemapSchema + "urlset", 
                    normalSitemapEntries.Cast<object>()
                                        .Concat(crowdactionEntries)
                                        .Concat(ExtraNamespaces)
                                        .ToArray()));
        }

        private XElement GetCrowdactionSitemapEntry(Crowdaction crowdaction)
        {
            decimal priority =
                crowdaction.DisplayPriority switch
                {
                    CrowdactionDisplayPriority.Top => 0.9m,
                    CrowdactionDisplayPriority.Medium => 0.7m,
                    _ => 0.5m
                };
            string changeFreq = crowdaction.IsClosed ? "monthly" : "hourly";
            XElement? videoSitemap = GetVideoSitemapEntry(crowdaction);
            XElement? imageSitemap = GetImageSitemapEntry(crowdaction);
            return GetUrlSitemapEntry(crowdaction.Url, changeFreq, priority, videoSitemap, imageSitemap);
        }

        private XElement? GetImageSitemapEntry(Crowdaction crowdaction)
        {
            if (crowdaction.BannerImage == null)
            {
                return null;
            }

            return new XElement(SitemapImageSchema + "image",
                new XElement(SitemapImageSchema + "loc", imageService.GetUrl(crowdaction.BannerImage)),
                new XElement(SitemapImageSchema + "caption", crowdaction.Name),
                new XElement(SitemapImageSchema + "title", crowdaction.Name));
        }

        private XElement? GetVideoSitemapEntry(Crowdaction crowdaction)
        {
            if (string.IsNullOrEmpty(crowdaction.DescriptionVideoLink))
            {
                return null;
            }

            string video = crowdaction.DescriptionVideoLink;
            string? videoId = GetVideoId(video);

            if (videoId == null)
            {
                return null;
            }

            return new XElement(SitemapVideoSchema + "video",
                new XElement(SitemapVideoSchema + "title", crowdaction.Name),
                new XElement(SitemapVideoSchema + "description", crowdaction.Name),
                new XElement(SitemapVideoSchema + "thumbnail_loc", new Uri($"https://img.youtube.com/vi/{videoId}/0.jpg")),
                new XElement(SitemapVideoSchema + "player_loc", new XAttribute("allow_embed", "yes"), video));
        }

        private string? GetVideoId(string video)
        {
            try
            {
                return video.LastIndexOf("?watch?v=", StringComparison.Ordinal) >= 0
                    ? video[(video.LastIndexOf('=') + 1)..]
                    : video[(video.LastIndexOf('/') + 1)..];
            }
            catch (ArgumentOutOfRangeException e)
            {
                logger.LogError(e, "Unable to retrieve video-id for {0}", video);
                return null;
            }
        }

        private XElement GetUrlSitemapEntry(Uri relativeUrl, string changeFrequency, decimal priority, params XElement?[] extraElements)
        {
            XElement[] standardUrlElements =
                new[]
                {
                   new XElement(SitemapSchema + "loc", new Uri(siteOptions.PublicUrl, relativeUrl)),
                   new XElement(SitemapSchema + "changefreq", changeFrequency),
                   new XElement(SitemapSchema + "priority", priority)
                };
            return new XElement(SitemapSchema + "url", standardUrlElements.Concat((XElement[])extraElements.Where(e => e != null)).ToArray());
        }
    }
}
