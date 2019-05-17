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

        public async Task<AddParticipantScenario> AddAnonymousParticipant(int projectId, string email)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project == null || !project.IsActive)
            {
                throw new ArgumentException($"{email} can't join project {projectId} because the project does not exist or is no longer active.");
            }

            var result = new AddParticipantResult();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var creationResult = await _userManager.CreateAsync(new ApplicationUser(email));
                if (!creationResult.Succeeded)
                {
                    var errors = string.Join(',', creationResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    throw new InvalidOperationException($"Could not create new unregisterd user {email}: {errors}");
                }

                result.UserCreated = true;
            }

            user = await _userManager.FindByEmailAsync(email);

            result.UserAdded = await InsertParticipant(projectId, user.Id);
            result.UserAlreadyActive = user.Activated;

            return result.Scenario;
        }

        public async Task<AddParticipantScenario> AddLoggedInParticipant(int projectId, string userId)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project == null || !project.IsActive)
            {
                throw new ArgumentException($"{userId} can't join project {projectId} because the project does not exist or is no longer active.");
            }

            var added = await InsertParticipant(projectId, userId);
            if (!added)
            {
                // This is not a valid scenario
                throw new ArgumentException($"User {userId} is already participating in project {projectId}.");
            }

            var result = new AddParticipantResult { LoggedIn = true, UserAdded = true };
            return result.Scenario;
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

    class AddParticipantResult
    {
        public bool LoggedIn { get; set;}
        public bool UserAdded {get; set;}
        public bool UserCreated { get; set; }
        public bool UserAlreadyActive { get; set; }

        public AddParticipantScenario Scenario 
        { 
            get 
            {
                if (LoggedIn && UserAdded) return AddParticipantScenario.LoggedInAndAdded;
                if (!LoggedIn && UserCreated && UserAdded) return AddParticipantScenario.AnonymousCreatedAndAdded;
                if (!LoggedIn && UserAlreadyActive && UserAdded) return AddParticipantScenario.AnonymousAlreadyRegisteredAndAdded;
                if (!LoggedIn && !UserAlreadyActive && UserAdded) return AddParticipantScenario.AnonymousNotRegisteredPresentAndAdded;
                if (!LoggedIn && UserAlreadyActive && !UserAdded) return AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating;
                if (!LoggedIn && !UserAlreadyActive && !UserAdded) return AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating;

                throw new InvalidOperationException(
                    "No participant scenario available for " +
                    $"{nameof(LoggedIn)}:{LoggedIn}, {nameof(UserAdded)}:{UserAdded}, " +
                    $"{nameof(UserCreated)}:{UserCreated}, {nameof(UserAlreadyActive)}:{UserAlreadyActive}");
            }
        }
    }
}
