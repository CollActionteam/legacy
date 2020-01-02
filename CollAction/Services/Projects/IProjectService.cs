using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CollAction.GraphQl.Mutations.Input;
using CollAction.Models;
using CollAction.Services.Projects.Models;

namespace CollAction.Services.Projects
{
    public interface IProjectService
    {
        Task<Project> CreateProject(NewProject newProject, ClaimsPrincipal user, CancellationToken token);

        Task<Project> UpdateProject(UpdatedProject updatedProject, ClaimsPrincipal user, CancellationToken token);

        Task<AddParticipantResult> CommitToProject(string email, int projectId, ClaimsPrincipal user, CancellationToken token);

        Task<Project> SendProjectEmail(int projectId, string subject, string message, ClaimsPrincipal performingUser, CancellationToken token);

        Task<ProjectParticipant> SetEmailProjectSubscription(int projectId, string userId, Guid token, bool isSubscribed, CancellationToken cancellationToken);

        bool CanSendProjectEmail(Project project);

        IQueryable<Project> SearchProjects(Category? category, SearchProjectStatus? status);
    }
}