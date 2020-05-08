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
        private static readonly string NumberActionsTakenKey = $"{typeof(StatisticsService).FullName}_{nameof(NumberActionsTaken)}";
        private static readonly string NumberCrowdactionsKey = $"{typeof(StatisticsService).FullName}_{nameof(NumberCrowdactions)}";
        private static readonly string NumberUsersKey = $"{typeof(StatisticsService).FullName}_{nameof(NumberUsers)}";
        private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(10);

        public StatisticsService(ApplicationDbContext context, IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
        }

        public Task<int> NumberActionsTaken(CancellationToken token)
        {
            return cache.GetOrCreateAsync(
                NumberActionsTakenKey, 
                (ICacheEntry entry) =>
                {
                    entry.SlidingExpiration = Expiration;
                    return context.CrowdactionParticipants
                                  .CountAsync(p =>
                                      p.Crowdaction!.End <= DateTime.UtcNow &&
                                      p.Crowdaction!.Status == CrowdactionStatus.Running &&
                                      p.Crowdaction!.ParticipantCounts!.Count + p.Crowdaction!.AnonymousUserParticipants >= p.Crowdaction!.Target, token);
                });
        }

        public Task<int> NumberCrowdactions(CancellationToken token)
        {
            return cache.GetOrCreateAsync(
                NumberCrowdactionsKey, 
                (ICacheEntry entry) =>
                {
                    entry.SlidingExpiration = Expiration;
                    return context.Crowdactions
                                  .CountAsync(p => p.Status == CrowdactionStatus.Running, token);
                });
        }

        public Task<int> NumberUsers(CancellationToken token)
        {
            return cache.GetOrCreateAsync(
                NumberUsersKey, 
                (ICacheEntry entry) =>
                {
                    entry.SlidingExpiration = Expiration;
                    return context.Users
                                  .CountAsync(u => u.Crowdactions.Any(), token);
                });
        }
    }
}
