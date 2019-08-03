using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CollAction.Models;
using CollAction.Services.Projects.Models;

namespace CollAction.Services.Projects
{
    public interface IProjectService
    {
        Task<Project> CreateProject(NewProject newProject, ClaimsPrincipal user, CancellationToken cancellationToken);

        Task<Project> UpdateProject(UpdatedProject updatedProject, ClaimsPrincipal user, CancellationToken cancellationToken);

        Task<AddParticipantResult> CommitToProject(string email, int projectId, ClaimsPrincipal user, CancellationToken cancellationToken);

        Task<Project> SendProjectEmail(int projectId, string subject, string message, ClaimsPrincipal performingUser, CancellationToken cancellationToken);

        Task<ProjectParticipant> SetProjectSubscription(int projectId, string userId, Guid token, bool isSubscribed, CancellationToken cancellationToken);

        bool CanSendProjectEmail(Project project);
    }
}