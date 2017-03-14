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
    }
}
