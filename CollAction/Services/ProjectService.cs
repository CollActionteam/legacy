using CollAction.Data;
using CollAction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project> GetProjectById(int? id)
        {
            return await _context.Projects.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetProjects(Expression<Func<Project, bool>> WhereExpression)
        {
            return await _context.Projects
                .Where(WhereExpression)
                .ToListAsync();
        }

        public async Task<bool> AddParticipant(string userId, int projectId)
        {
            var participant = new ProjectParticipant
            {
                UserId = userId,
                ProjectId = projectId
            };

            try
            {
                _context.Add(participant);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public async Task<ProjectParticipant> GetParticipant(string userId, int projectId)
        {
            return await _context.ProjectParticipants.SingleOrDefaultAsync(p => p.ProjectId == projectId && p.UserId == userId);
        }

        public async Task<IEnumerable<TileProject>> GetTileProjects(Expression<Func<Project, bool>> WhereExpression)
        {
            return await _context.Projects
               .Where(WhereExpression)
               .Select(p =>
                new TileProject
                {
                    ProjectId = p.Id,
                    ProjectName = p.Name,
                    ProjectProposal = p.Proposal,
                    CategoryName = p.Category.Name,
                    CategoryColorHex = p.Category.ColorHex,
                    LocationName = p.Location.Name,
                    ProjectStart = p.Start,
                    ProjectEnd = p.End,
                    BannerImage = p.BannerImage,
                    Target = p.Target,
                    Participants = p.Participants.Count(),
                    ProjectStatus = p.Status,
                    IsActive = p.IsActive,
                    IsComingSoon = p.IsComingSoon,
                    IsClosed = p.IsClosed,
                    DisplayPriority = p.DisplayPriority
                })
               .OrderBy(t => t.DisplayPriority)
               .ToListAsync();
        }
    }

    public class TileProject
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectProposal { get; set; }

        public string CategoryName { get; set; }

        public string CategoryColorHex { get; set; }

        public string LocationName { get; set; }

        public DateTime ProjectStart { get; set; }

        public DateTime ProjectEnd { get; set; }

        public TimeSpan RemainingTime
        {
            get
            {
                if (DateTime.UtcNow >= ProjectEnd || ProjectEnd <= ProjectStart)
                    return TimeSpan.Zero;
                return ProjectEnd - (DateTime.UtcNow > ProjectStart ? DateTime.UtcNow : ProjectStart);
            }
        }

        public ImageFile BannerImage { get; set; }

        public int Target { get; set; }

        public int Participants { get; set; }

        public int ProgressPercent
        {
            get
            {
                return Math.Min(Participants * 100 / Target, 100);
            }
        }

        public ProjectStatus ProjectStatus { get; set; }

        public bool IsActive { get; set; }

        public bool IsComingSoon { get; set; }

        public bool IsClosed { get; set; }

        public ProjectDisplayPriority DisplayPriority { get; set; }

    }
}