using CollAction.Data;
using CollAction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Statistics
{
    public sealed class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext context;

        public StatisticsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task<int> NumberActionsTaken(CancellationToken token)
            => context.CrowdactionParticipants
                      .CountAsync(p => 
                          p.Crowdaction.End <= DateTime.UtcNow && 
                          p.Crowdaction.Status == CrowdactionStatus.Running && 
                          p.Crowdaction.ParticipantCounts!.Count + p.Crowdaction.AnonymousUserParticipants >= p.Crowdaction.Target, token);

        public Task<int> NumberCrowdactions(CancellationToken token)
            => context.Crowdactions
                      .CountAsync(p => p.Status == CrowdactionStatus.Running, token);

        public Task<int> NumberUsers(CancellationToken token)
            => context.Users
                      .CountAsync(u => u.Crowdactions.Any(), token);
    }
}
