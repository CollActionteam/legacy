using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CollAction.Services.Sitemap
{
    public interface ISitemapService
    {
        Task<XDocument> GetSitemap(CancellationToken cancellationToken);
    }
}
