using CollAction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public interface IProjectService
    {
        Task<Project> GetProjectById(int id);
        Task<bool> AddParticipant(string userId, int projectId);
        Task<ProjectParticipant> GetParticipant(string userId, int projectId);
        Task<string> GenerateParticipantsDataExport(int projectId);
        bool CanSendProjectEmail(Project project);
        Task SendProjectEmail(Project project, string subject, string message, HttpRequest request, IUrlHelper helper);
        Task<IEnumerable<DisplayProjectViewModel>> GetProjectDisplayViewModels(Expression<Func<Project, bool>> filter);
        Task<IEnumerable<FindProjectsViewModel>> FindProjects(Expression<Func<Project, bool>> filter);
        int NumberEmailsAllowedToSend(Project project);
        DateTime CanSendEmailsUntil(Project project);
        string GetProjectNameNormalized(string projectName);
    }
}
