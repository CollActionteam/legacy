using CollAction.Data;
using CollAction.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<IProjectService> _localizer;

        public ProjectService(ApplicationDbContext context, IStringLocalizer<IProjectService> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Project> GetProjectById(int? id)
        {
            return await _context.Projects.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project> GetProjectByName(string name)
        {
            return await _context.Projects.SingleOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<Project>> GetProjects(Expression<Func<Project, bool>> WhereExpression)
        {
            return await _context.Projects
                .Where(WhereExpression)
                .ToListAsync();
        }

        public async Task<bool> AddParticipant(string userId, int projectId)
        {
            Project project = await GetProjectById(projectId);
            if (project == null || !project.IsActive)
            {
                return false;
            }

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

        public async Task<IEnumerable<DisplayTileProjectViewModel>> GetTileProjects(Expression<Func<Project, bool>> WhereExpression)
        {
            return await _context.Projects
               .Where(WhereExpression)
               .OrderBy(p => p.DisplayPriority)
               .Select(p =>
                new DisplayTileProjectViewModel(_localizer, p.Start, p.End, p.Status, p.IsActive, p.IsComingSoon, p.IsClosed)
                {
                    ProjectId = p.Id,
                    ProjectName = p.Name,
                    ProjectNameUrl = WebUtility.UrlEncode(p.Name),
                    ProjectProposal = p.Proposal,
                    CategoryName = p.Category.Name,
                    CategoryColorHex = p.Category.ColorHex,
                    LocationName = p.Location.Name,
                    BannerImagePath = p.BannerImage != null ? p.BannerImage.Filepath : $"/images/default_banners/{p.Category.Name}.jpg",
                    BannerImageDescription = p.BannerImage.Description,
                    Target = p.Target,
                    Participants = p.Participants.Sum(participant => participant.User.RepresentsNumberParticipants)
                })
               .ToListAsync();
        }

        public async Task<string> GenerateParticipantsDataExport(int projectId)
        {
            Project project = await _context
                .Projects
                .Where(p => p.Id == projectId)
                .Include(p => p.Participants).ThenInclude(p => p.User)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync();
            if (project == null)
                return null;
            else
                return string.Join("\r\n", GetParticipantsCsv(project));
        }

        private IEnumerable<string> GetParticipantsCsv(Project project)
        {
            yield return "first-name;last-name;email";
            yield return GetParticipantCsvLine(project.Owner);
            foreach (ProjectParticipant participant in project.Participants)
                yield return GetParticipantCsvLine(participant.User);
        }

        private string GetParticipantCsvLine(ApplicationUser user)
            => $"{EscapeCsv(user.FirstName)};{EscapeCsv(user.LastName)};{EscapeCsv(user.Email)}";

        private string EscapeCsv(string str)
            => $"\"{str?.Replace("\"", "\"\"")}\"";
    }
}