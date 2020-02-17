using CollAction.Data;
using CollAction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext context;

        public StatisticsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task<int> NumberActionsTaken(CancellationToken token)
            => context.ProjectParticipants
                      .CountAsync(p => 
                          p.Project.End <= DateTime.UtcNow && 
                          p.Project.Status == ProjectStatus.Running && 
                          p.Project.ParticipantCounts!.Count + p.Project.AnonymousUserParticipants >= p.Project.Target, token);

        public Task<int> NumberProjects(CancellationToken token)
            => context.Projects
                      .CountAsync(p => p.Status == ProjectStatus.Running, token);

        public Task<int> NumberUsers(CancellationToken token)
            => context.Users
                      .CountAsync(u => u.Projects.Any(), token);
    }
}
