using CollAction.Data;
using CollAction.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Statistics
{
    public sealed class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext context;
        private readonly IMemoryCache cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);
        private static readonly string NumberActionsTakenKey = $"{typeof(StatisticsService).FullName}_{nameof(NumberActionsTaken)}";
        private static readonly string NumberCrowdactionsKey = $"{typeof(StatisticsService).FullName}_{nameof(NumberCrowdactions)}";
        private static readonly string NumberUsersKey = $"{typeof(StatisticsService).FullName}_{nameof(NumberUsers)}";

        public StatisticsService(ApplicationDbContext context, IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
        }

        public Task<int> NumberActionsTaken(CancellationToken token)
            => cache.GetOrCreateAsync(
                   NumberActionsTakenKey,
                   async (ICacheEntry entry) =>
                   {
                       entry.SlidingExpiration = CacheExpiration;

                       int normalParticipantCount =
                           await context.CrowdactionParticipants
                                        .CountAsync(c =>
                                            c.Crowdaction!.End <= DateTime.UtcNow &&
                                            c.Crowdaction!.Status == CrowdactionStatus.Running &&
                                            c.Crowdaction!.ParticipantCounts!.Count + c.Crowdaction!.AnonymousUserParticipants >= c.Crowdaction!.Target, token).ConfigureAwait(false);
                       int anonymousParticipantCount =
                                    await context.Crowdactions
                                                 .Where(c => c.ParticipantCounts!.Count + c.AnonymousUserParticipants >= c.Target)
                                                 .SumAsync(c => c.AnonymousUserParticipants, token)
                                                 .ConfigureAwait(false);

                       return normalParticipantCount + anonymousParticipantCount;
                   });

        public Task<int> NumberCrowdactions(CancellationToken token)
            => cache.GetOrCreateAsync(
                   NumberCrowdactionsKey,
                   (ICacheEntry entry) =>
                   {
                       entry.SlidingExpiration = CacheExpiration;
                       return context.Crowdactions
                                     .CountAsync(c => c.Status == CrowdactionStatus.Running, token);
                   });

        public Task<int> NumberUsers(CancellationToken token)
            => cache.GetOrCreateAsync(
                   NumberUsersKey,
                   async (ICacheEntry entry) =>
                   {
                       entry.SlidingExpiration = CacheExpiration;

                       int normalUsers = 
                           await context.Users
                                        .CountAsync(u => u.Crowdactions.Any(), token)
                                        .ConfigureAwait(false);
                       int anonymousUsers =
                           await context.Crowdactions
                                        .SumAsync(c => c.AnonymousUserParticipants)
                                        .ConfigureAwait(false);

                       return normalUsers + anonymousUsers;
                   });
    }
}
