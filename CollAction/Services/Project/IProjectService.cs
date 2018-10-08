using CollAction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CollAction.Services.Project
{
    public interface IProjectService
    {
        Task<Models.Project> GetProjectById(int id);
        Task<bool> AddParticipant(string userId, int projectId);
        Task<ProjectParticipant> GetParticipant(string userId, int projectId);
        Task<string> GenerateParticipantsDataExport(int projectId);
        bool CanSendProjectEmail(Models.Project project);
        Task SendProjectEmail(Models.Project project, string subject, string message, HttpRequest request, IUrlHelper helper);
        Task<IEnumerable<DisplayProjectViewModel>> GetProjectDisplayViewModels(Expression<Func<Models.Project, bool>> filter);
        Task<IEnumerable<FindProjectsViewModel>> FindProjects(Expression<Func<Models.Project, bool>> filter);
        int NumberEmailsAllowedToSend(Models.Project project);
        DateTime CanSendEmailsUntil(Models.Project project);
        Task RefreshParticipantCountMaterializedView();
    }
}