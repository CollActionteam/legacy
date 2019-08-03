﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CollAction.Models;
using CollAction.Services.Projects.Models;

namespace CollAction.Services.Projects
{
    public interface IProjectService
    {
        Task<Project> CreateProject(NewProject newProject, ClaimsPrincipal user);

        Task<Project> UpdateProject(UpdatedProject updatedProject, ClaimsPrincipal user);

        Task<AddParticipantResult> CommitToProject(string email, int projectId, ClaimsPrincipal user);

        Task<Project> SendProjectEmail(int projectId, string subject, string message, ClaimsPrincipal performingUser);

        Task<ProjectParticipant> SetProjectSubscription(int projectId, string userId, Guid token, bool isSubscribed);

        bool CanSendProjectEmail(Project project);
    }
}