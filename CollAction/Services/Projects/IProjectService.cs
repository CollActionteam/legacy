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
        Task<ProjectResult> CreateProject(NewProject newProject, ClaimsPrincipal user, CancellationToken token);

        Task<Project> CreateProjectInternal(NewProjectInternal newProject, CancellationToken token);

        Task<ProjectResult> UpdateProject(UpdatedProject updatedProject, ClaimsPrincipal user, CancellationToken token);

        Task<int> DeleteProject(int id, CancellationToken token);

        Task<AddParticipantResult> CommitToProjectAnonymous(string email, int projectId, CancellationToken token);

        Task<AddParticipantResult> CommitToProjectLoggedIn(ClaimsPrincipal user, int projectId, CancellationToken token);

        Task<ProjectResult> SendProjectEmail(int projectId, string subject, string message, ClaimsPrincipal performingUser, CancellationToken token);

        Task<ProjectParticipant> SetEmailProjectSubscription(int projectId, string userId, Guid token, bool isSubscribed, CancellationToken cancellationToken);

        bool CanSendProjectEmail(Project project);

        IQueryable<Project> SearchProjects(Category? category, SearchProjectStatus? status);

        Task RefreshParticipantCount(CancellationToken token);

        void InitializeRefreshParticipantCountJob();
    }
}