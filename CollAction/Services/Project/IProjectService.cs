using CollAction.Models;
using CollAction.Models.ProjectViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CollAction.Services.Project
{
    public interface IProjectService
    {
        Task<Models.Project> GetProjectById(int id);
        bool CanSendProjectEmail(Models.Project project);
        Task SendProjectEmail(Models.Project project, string subject, string message, HttpRequest request, IUrlHelper helper);
        IQueryable<DisplayProjectViewModel> GetProjectDisplayViewModels(Expression<Func<Models.Project, bool>> filter);
        int NumberEmailsAllowedToSend(Models.Project project);
        DateTime CanSendEmailsUntil(Models.Project project);
        string GetProjectNameNormalized(string projectName);
        Task<FindProjectsViewModel> FindProject(int projectId);
        IQueryable<FindProjectsViewModel> FindProjects(Expression<Func<Models.Project, bool>> filter, int? limit, int? start);
        Task<IEnumerable<FindProjectsViewModel>> MyProjects(string userId);
        Task<IEnumerable<FindProjectsViewModel>> ParticipatingInProjects(string userId);
        Task<bool> ToggleNewsletterSubscription(int projectId, string userId);
    }
}
