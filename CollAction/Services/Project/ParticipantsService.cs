using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollAction.Services.Project
{
    public class ParticipantsService : IParticipantsService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParticipantsService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddUnregisteredParticipant(int projectId, string email, Uri projectLink)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project == null || !project.IsActive)
            {
                return;
            }

            var result = await _userManager.CreateAsync(new ApplicationUser(email));

            if (!result.Succeeded)
            {
                var errors = string.Join(',', result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new InvalidOperationException($"Could not create new unregisterd user {email}: {errors}");
            }

            var user = await _userManager.FindByEmailAsync(email);
            var added = await InsertParticipant(projectId, user.Id);

            if (added) 
            {
                // Send welcome e-mail incl. invitation to finish registration
            }
            else 
            {
                // Send e-mail that you are already participating... (and link to login screen)
            }
        }

        public async Task AddParticipant(int projectId, string userId, Uri projectLink)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project == null || !project.IsActive)
            {
                return;
            }

            var added = await InsertParticipant(projectId, userId);

            if (added) 
            {
                // Send welcome e-mail
            }
            else 
            {
                // Send e-mail that you are already participating... (and link to login screen)
            }
        }

        private async Task<bool> InsertParticipant(int projectId, string userId)
        {
            var participant = new ProjectParticipant
            {
                UserId = userId,
                ProjectId = projectId,
                SubscribedToProjectEmails = true,
                UnsubscribeToken = Guid.NewGuid()
            };

            try
            {
                _context.Add(participant);
                
                await _context.SaveChangesAsync();
                
                await RefreshParticipantCountMaterializedView();
                
                return true;
            }
            catch (DbUpdateException)
            {
                // User is already participanting
                return false;
            }            
        }    

        public Task RefreshParticipantCountMaterializedView()
            => _context.Database.ExecuteSqlCommandAsync(@"REFRESH MATERIALIZED VIEW CONCURRENTLY ""ProjectParticipantCounts"";");

        public async Task<ProjectParticipant> GetParticipant(string userId, int projectId)
        {
            return await _context.ProjectParticipants.SingleOrDefaultAsync(p => p.ProjectId == projectId && p.UserId == userId);
        }

        public async Task<string> GenerateParticipantsDataExport(int projectId)
        {
            Models.Project project = await _context
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

        private IEnumerable<string> GetParticipantsCsv(Models.Project project)
        {
            yield return "first-name;last-name;email";
            yield return GetParticipantCsvLine(project.Owner);
            foreach (ProjectParticipant participant in project.Participants)
            {
                yield return GetParticipantCsvLine(participant.User);
            }
        }

        private string GetParticipantCsvLine(ApplicationUser user)
            => $"{EscapeCsv(user?.FirstName)};{EscapeCsv(user?.LastName)};{EscapeCsv(user?.Email)}";

        private string EscapeCsv(string str)
            => $"\"{str?.Replace("\"", "\"\"")}\"";
    }
}