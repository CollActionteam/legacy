using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CollAction.Helpers
{
    public class SitemapHelper
    {
        private static XNamespace _urlsetNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static XNamespace _imageNamespace = "http://www.google.com/schemas/sitemap-image/1.1";
        private const string _protocol = "https://";
        private readonly ApplicationDbContext _context;
        private readonly IUrlHelper _urlHelper;

        public SitemapHelper(ApplicationDbContext context, IUrlHelper urlHelper)
        {
            _context = context;
            _urlHelper = urlHelper;
        }

        public async Task<XDocument> GetSitemap()
        {
            HostString host = _urlHelper.ActionContext.HttpContext.Request.Host;
            object[] homepageUrls = new[]
            {
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", _protocol + host + _urlHelper.Action("Index", "Home"))),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", _protocol + host + _urlHelper.Action("About", "Home"))),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", _protocol + host + _urlHelper.Action("CrowdActingFestival", "Home"))),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", _protocol + host + _urlHelper.Action("Contact", "Home"))),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", _protocol + host + _urlHelper.Action("FAQ", "Home"))),
                new XElement(_urlsetNamespace + "url", new XElement(_urlsetNamespace + "loc", _protocol + host + _urlHelper.Action("Find", "Projects")))
            };

            object[] projectUrls = await _context.Projects
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

        private XElement GetSitemapProjectEntry(Project project)
        {
            HostString host = _urlHelper.ActionContext.HttpContext.Request.Host;
            List<XElement> projectElements = new List<XElement>(3)
            {
                new XElement(_urlsetNamespace + "loc", _protocol + host + _urlHelper.Action("Details", "Projects", new { name = project.Name}) )
            };
            if (project.BannerImageFileId != null)
            {
                projectElements.Add(new XElement(_imageNamespace + "image", new[]
                {
                    new XElement(_imageNamespace + "loc", _protocol + host + _urlHelper.Content(project.BannerImage.Filepath)),
                    new XElement(_imageNamespace + "caption", project.BannerImage.Description)
                }));
            }
            return new XElement(_urlsetNamespace + "url", projectElements.ToArray());
        }
    }
}
