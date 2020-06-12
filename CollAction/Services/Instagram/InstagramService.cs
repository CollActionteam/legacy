using CollAction.Services.Instagram.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Instagram
{
    public sealed class InstagramService : IInstagramService
    {
        private readonly IMemoryCache cache;
        private readonly HttpClient instagramClient;
        private readonly ILogger<InstagramService> logger;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

        public InstagramService(IMemoryCache cache, HttpClient instagramClient, ILogger<InstagramService> logger)
        {
            this.cache = cache;
            this.instagramClient = instagramClient;
            this.logger = logger;
        }

        public async Task<IEnumerable<InstagramWallItem>> GetItems(string instagramUser, CancellationToken token)
        {
            // TODO: Stop using the instagram private api
            Uri url = new Uri($"/{instagramUser}/?__a=1", UriKind.Relative);
            try
            {
                return await cache.GetOrCreateAsync(
                                 url,
                                 async (ICacheEntry entry) =>
                                 {
                                     entry.SlidingExpiration = CacheExpiration;
                                     logger.LogInformation("Retrieving instagram timeline for {0}", instagramUser);
                                     var result = await instagramClient.GetAsync(url, token).ConfigureAwait(false);
                                     result.EnsureSuccessStatusCode();
                                     return ParseInstagramResponse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
                                 }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error retrieving items from instagram timeline");
                return Enumerable.Empty<InstagramWallItem>(); // External APIs can fail here, lets be a little robust here. Don't cache the failures though.. so catch outside the cache
            }
        }

        private IEnumerable<InstagramWallItem> ParseInstagramResponse(string json)
        {
            dynamic deserialized = JsonConvert.DeserializeObject<dynamic>(json);
            var edges = (IEnumerable<dynamic>)(deserialized.graphql?.user?.edge_owner_to_timeline_media?.edges ?? Enumerable.Empty<dynamic>());
            logger.LogInformation("Retrieving instagram timeline, found {0} items", edges.Count());
            return edges
                .Select(e => e.node)
                .Where(e => e.shortcode != null && e.thumbnail_src != null && e.taken_at_timestamp != null)
                .Select(e =>
                {
                    var captionEdges = (IEnumerable<dynamic>?)e.edge_media_to_caption?.edges;
                    string? caption = (string?)captionEdges?.FirstOrDefault()?.node?.text;
                    return new InstagramWallItem(
                        (string)e.shortcode,
                        (string)e.thumbnail_src,
                        (string?)e.accessibility_caption,
                        caption,
                        DateTimeOffset.FromUnixTimeSeconds((long)e.taken_at_timestamp));
                });
        }
    }
}
