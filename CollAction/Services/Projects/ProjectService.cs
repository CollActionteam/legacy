﻿using CollAction.Data;
using CollAction.Services.Email;
using CollAction.Services.Projects.Models;
using CollAction.ViewModels.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CollAction.Models;
using CollAction.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly IEmailSender emailSender;
        private readonly ILogger<ProjectService> logger;
        private readonly ProjectEmailOptions projectEmailOptions;
        private readonly SiteOptions siteOptions;
        private readonly IHtmlInputValidator htmlInputValidator;

        public ProjectService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<ProjectService> logger,
            IOptions<ProjectEmailOptions> projectEmailOptions,
            IOptions<SiteOptions> siteOptions,
            IHtmlInputValidator htmlInputValidator)
        {
            this.userManager = userManager;
            this.context = context;
            this.emailSender = emailSender;
            this.logger = logger;
            this.projectEmailOptions = projectEmailOptions.Value;
            this.siteOptions = siteOptions.Value;
            this.htmlInputValidator = htmlInputValidator;
        }

        public async Task<Project> CreateProject(NewProject newProject, ClaimsPrincipal user)
        {
            ApplicationUser owner = await userManager.GetUserAsync(user);
            if (owner == null)
            {
                throw new ValidationException("User not found");
            }

            var project = new Project
            {
                OwnerId = owner.Id,
                Name = newProject.Name,
                Description = newProject.Description,
                Proposal = newProject.Proposal,
                Goal = newProject.Goal,
                CreatorComments = newProject.CreatorComments,
                CategoryId = newProject.CategoryId,
                Target = newProject.Target,
                Start = newProject.Start,
                End = newProject.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                BannerImageFileId = newProject.BannerImageFileId,
                DescriptiveImageFileId = newProject.DescriptiveImageFileId,
                DescriptionVideoLink = newProject.DescriptionVideoLink
            };

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            if (newProject.Tags.Any())
            {
                IEnumerable<string> missingTags = newProject.Tags.Except(
                    await context.Tags
                                 .Where(t => newProject.Tags.Contains(t.Name))
                                 .Select(t => t.Name)
                                 .ToListAsync());

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags.Select(t => new Tag() { Name = t }));
                    await context.SaveChangesAsync();
                }

                Dictionary<string, int> tags = await context.Tags
                                        .Where(t => newProject.Tags.Contains(t.Name))
                                        .ToDictionaryAsync(t => t.Name, t => t.Id);
                IEnumerable<ProjectTag> projectTags =
                    newProject.Tags
                              .Select(t => new ProjectTag() { ProjectId = project.Id, TagId = tags[t] });
                context.ProjectTags.AddRange(projectTags);
                await context.SaveChangesAsync();
            }

            await RefreshParticipantCountMaterializedView();

            await emailSender.SendEmailTemplated(owner.Email, $"Thank you for submitting \"{project.Name}\" on CollAction", "ProjectConfirmation");

            IList<ApplicationUser> administrators = await userManager.GetUsersInRoleAsync(Constants.AdminRole);
            await emailSender.SendEmailsTemplated(administrators.Select(a => a.Email), $"A new project was submitted on CollAction: \"{project.Name}\"", "ProjectAddedAdmin", project.Name);

            return project;
        }

        public async Task<Project> UpdateProject(UpdatedProject updatedProject, ClaimsPrincipal user)
        {
            if (user.IsInRole(Constants.AdminRole))
            {
                throw new ValidationException("Not allowed to update project");
            }

            bool approved = updatedProject.Status == ProjectStatus.Running && updatedProject.Status == ProjectStatus.Hidden;
            bool successfull = updatedProject.Status == ProjectStatus.Successfull && updatedProject.Status == ProjectStatus.Running;
            bool failed = updatedProject.Status == ProjectStatus.Failed && updatedProject.Status == ProjectStatus.Running;
            bool deleted = updatedProject.Status == ProjectStatus.Deleted;

            Project project = await context
                .Projects
                .Include(p => p.Tags).ThenInclude(t => t.Tag)
                .FirstOrDefaultAsync(p => p.Id == updatedProject.Id);

            if (project == null)
            {
                throw new ValidationException("Project not found");
            }

            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;
            project.Proposal = updatedProject.Proposal;
            project.Goal = updatedProject.Goal;
            project.CreatorComments = updatedProject.CreatorComments;
            project.CategoryId = updatedProject.CategoryId;
            project.Target = updatedProject.Target;
            project.Start = updatedProject.Start;
            project.End = updatedProject.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            project.BannerImageFileId = updatedProject.BannerImageFileId;
            project.DescriptiveImageFileId = updatedProject.DescriptiveImageFileId;
            project.DescriptionVideoLink = updatedProject.DescriptionVideoLink;
            project.Status = updatedProject.Status;
            project.DisplayPriority = updatedProject.DisplayPriority;
            project.NumberProjectEmailsSend = updatedProject.NumberProjectEmailsSend;
            context.Projects.Update(project);
            await context.SaveChangesAsync();

            var projectTags = project.Tags.Select(t => t.Tag.Name);
            if (!Enumerable.SequenceEqual(updatedProject.Tags.OrderBy(t => t), projectTags.OrderBy(t => t)))
            {
                IEnumerable<string> missingTags = updatedProject.Tags.Except(
                    await context.Tags
                                 .Where(t => updatedProject.Tags.Contains(t.Name))
                                 .Select(t => t.Name)
                                 .ToListAsync());

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags.Select(t => new Tag() { Name = t }));
                    await context.SaveChangesAsync();
                }

                Dictionary<string, int> tags = await context.Tags
                                        .Where(t => updatedProject.Tags.Contains(t.Name) || projectTags.Contains(t.Name))
                                        .ToDictionaryAsync(t => t.Name, t => t.Id);

                IEnumerable<ProjectTag> newTags =
                    updatedProject.Tags
                                  .Except(projectTags)
                                  .Select(t => new ProjectTag() { ProjectId = project.Id, TagId = tags[t] });
                IEnumerable<ProjectTag> removedTags =
                    project.Tags.Where(t => projectTags.Except(updatedProject.Tags).Contains(t.Tag.Name));
                context.ProjectTags.AddRange(newTags);
                context.ProjectTags.RemoveRange(removedTags);
                await context.SaveChangesAsync();
            }

            ApplicationUser owner = await userManager.FindByIdAsync(project.OwnerId);

            if (approved)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Approval - {project.Name}", "ProjectApproval");
            }
            else if (successfull)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Success - {project.Name}", "ProjectSuccess");
            }
            else if (failed)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Failed - {project.Name}", "ProjectFailed");
            }
            else if (deleted)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Deleted - {project.Name}", "ProjectDeleted");
            }

            return project;
        }

        public async Task<Project> SendProjectEmail(int projectId, string subject, string message, ClaimsPrincipal performingUser)
        {
            Project project = await context.Projects.FindAsync(projectId);
            if (project == null)
            {
                throw new ValidationException("Project not found");
            }

            if (!(htmlInputValidator.IsSafe(message) && htmlInputValidator.IsSafe(subject)))
            {
                throw new ValidationException("Unsafe html");
            }

            ApplicationUser user = await userManager.GetUserAsync(performingUser);
            if (project.OwnerId != user.Id)
            {
                throw new InvalidOperationException("Unauthorized");
            }

            IEnumerable<ProjectParticipant> participants =
                await context.ProjectParticipants
                             .Include(part => part.User)
                             .Where(part => part.ProjectId == project.Id && part.SubscribedToProjectEmails)
                             .ToListAsync();

            logger.LogInformation("sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());

            foreach (ProjectParticipant participant in participants)
            {
                string unsubscribeLink = $"{siteOptions.PublicAddress}/#/ChangeSubscription?userId={WebUtility.UrlEncode(participant.UserId)}&projectId={project.Id}&token={WebUtility.UrlEncode(participant.UnsubscribeToken.ToString())}";
                emailSender.SendEmail(participant.User.Email, subject, FormatEmailMessage(message, participant.User, unsubscribeLink));
            }

            IList<ApplicationUser> adminUsers = await userManager.GetUsersInRoleAsync(Constants.AdminRole);
            foreach (ApplicationUser admin in adminUsers)
            {
                emailSender.SendEmail(admin.Email, subject, FormatEmailMessage(message, admin, null));
            }

            emailSender.SendEmail(project.Owner.Email, subject, FormatEmailMessage(message, project.Owner, null));

            project.NumberProjectEmailsSend += 1;
            await context.SaveChangesAsync();

            logger.LogInformation("done sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());
            return project;
        }

        public bool CanSendProjectEmail(Project project)
        {
            DateTime now = DateTime.UtcNow;
            return project.NumberProjectEmailsSend < projectEmailOptions.MaxNumberProjectEmails &&
                   project.End + projectEmailOptions.TimeEmailAllowedAfterProjectEnd > now &&
                   project.Status != ProjectStatus.Deleted &&
                   project.Status != ProjectStatus.Hidden &&
                   project.Status != ProjectStatus.Failed &&
                   now >= project.Start;
        }

        public async Task<ProjectParticipant> SetProjectSubscription(int projectId, string userId, Guid token, bool isSubscribed)
        {
            ProjectParticipant participant = await context
                 .ProjectParticipants
                 .Include(p => p.Project)
                 .FirstAsync(p => p.ProjectId == projectId && p.UserId == userId);

            if (participant != null && participant.UnsubscribeToken == token)
            {
                participant.SubscribedToProjectEmails = isSubscribed;
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Not authorized");
            }

            return participant;
        }

        public async Task<AddParticipantResult> CommitToProject(string email, int projectId, ClaimsPrincipal user)
        {
            ApplicationUser applicationUser = await userManager.GetUserAsync(user);
            if (applicationUser == null && string.IsNullOrEmpty(email))
            {
                return new AddParticipantResult()
                {
                    Error = "E-mail address not valid"
                };
            }

            Project project = await context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return new AddParticipantResult()
                {
                    Error = "Project not found"
                };
            }

            AddParticipantResult result = applicationUser != null
                ? await AddLoggedInParticipant(project, applicationUser)
                : await AddAnonymousParticipant(project, applicationUser, email);

            if (result.Scenario != AddParticipantScenario.Error &&
                result.Scenario != AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating &&
                result.Scenario != AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating)
            {
                var commitEmailViewModel = new ProjectCommitEmailViewModel()
                {
                    Project = project,
                    Result = result,
                    User = applicationUser,
                    PublicAddress = siteOptions.PublicAddress,
                    ProjectUrl = $"https://{siteOptions.PublicAddress}/{project.Url}"
                };

                await emailSender.SendEmailTemplated(email, $"Thank you for participating in the \"{project.Name}\" project on CollAction", "ProjectCommit", commitEmailViewModel);
            }

            return result;
        }

        private async Task<AddParticipantResult> AddAnonymousParticipant(Project project, ApplicationUser user, string email)
        {
            var result = new AddParticipantResult();
            if (user == null)
            {
                user = new ApplicationUser(email);
                IdentityResult creationResult = await userManager.CreateAsync(user);
                if (!creationResult.Succeeded)
                {
                    string errors = string.Join(',', creationResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    return new AddParticipantResult()
                    {
                        Error = $"Could not create new unregistered user {email}: {errors}"
                    };
                }

                result.UserCreated = true;
            }

            result.UserAdded = await InsertParticipant(project.Id, user.Id);
            result.UserAlreadyActive = user.Activated;

            if (!result.UserAlreadyActive)
            {
                result.ParticipantEmail = user.Email;
                result.PasswordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            }

            return result;
        }

        private async Task<AddParticipantResult> AddLoggedInParticipant(Project project, ApplicationUser user)
        {
            bool added = await InsertParticipant(project.Id, user.Id);
            if (!added)
            {
                // This is not a valid scenario
                return new AddParticipantResult()
                {
                    Error = $"User {user.Id} is already participating in project {project.Name}."
                };
            }

            return new AddParticipantResult { LoggedIn = true, UserAdded = true };
        }

        private Task RefreshParticipantCountMaterializedView()
            => context.Database.ExecuteSqlCommandAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY \"ProjectParticipantCounts\";");

        private string FormatEmailMessage(string message, ApplicationUser user, string unsubscribeLink)
        {
            if (!string.IsNullOrEmpty(unsubscribeLink))
            {
                message = message + $"<br><a href=\"{unsubscribeLink}\">Unsubscribe</a>";
            }

            if (string.IsNullOrEmpty(user.FirstName))
            {
                message = message.Replace(" {firstname}", string.Empty)
                                 .Replace("{firstname}", string.Empty);
            }
            else
            {
                message = message.Replace("{firstname}", user.FirstName);
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                message = message.Replace(" {lastname}", string.Empty)
                                 .Replace("{lastname}", string.Empty);
            }
            else
            {
                message = message.Replace("{lastname}", user.LastName);
            }

            return message;
        }

        private async Task<bool> InsertParticipant(int projectId, string userId)
        {
            ProjectParticipant participant = new ProjectParticipant
            {
                UserId = userId,
                ProjectId = projectId,
                SubscribedToProjectEmails = true,
                UnsubscribeToken = Guid.NewGuid()
            };

            try
            {
                context.ProjectParticipants.Add(participant);

                await context.SaveChangesAsync();

                await RefreshParticipantCountMaterializedView();

                return true;
            }
            catch (DbUpdateException)
            {
                // User is already participanting
                return false;
            }
        }
    }
}
