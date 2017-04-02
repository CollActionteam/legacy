using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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

        public async Task<IEnumerable<DisplayTileProjectViewModel>> GetTileProjects(IUrlHelper urlHelper, Expression<Func<Project, bool>> WhereExpression)
        {
            return _context.Projects
               .Where(WhereExpression)
               .OrderBy(p => p.DisplayPriority)
               .Select(p =>
                new DisplayTileProjectViewModel(_localizer, p.Start, p.End, p.Status, p.IsActive, p.IsComingSoon, p.IsClosed)
                {
                    ProjectId = p.Id,
                    ProjectName = p.Name,
                    ProjectProposal = p.Proposal,
                    CategoryName = p.Category.Name,
                    CategoryColorHex = p.Category.ColorHex,
                    LocationName = p.Location.Name,
                    BannerImagePath = GetImagePath(urlHelper, p.BannerImage, "https://placeholdit.imgix.net/~text?txtsize=33&txt=Project%20Image"),
                    Target = p.Target,
                    Participants = p.Participants.Count()
                })
               .ToList();
        }

        public string GetImagePath(IUrlHelper url, ImageFile imageFile, string defaultImagePath)
        {
            return (imageFile != null) ? url.Content(imageFile.Filepath.Replace('\\', '/')) : defaultImagePath;
        }
    }
}