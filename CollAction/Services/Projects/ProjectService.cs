using CollAction.Data;
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
using System.ComponentModel.DataAnnotations;
using System.Threading;
using CollAction.Services.HtmlValidator;
using CollAction.GraphQl.Mutations.Input;
using Hangfire;
using System.Text.RegularExpressions;

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
        private readonly IBackgroundJobClient jobClient;

        public ProjectService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<ProjectService> logger,
            IOptions<ProjectEmailOptions> projectEmailOptions,
            IOptions<SiteOptions> siteOptions,
            IHtmlInputValidator htmlInputValidator,
            IBackgroundJobClient jobClient)
        {
            this.userManager = userManager;
            this.context = context;
            this.emailSender = emailSender;
            this.logger = logger;
            this.projectEmailOptions = projectEmailOptions.Value;
            this.siteOptions = siteOptions.Value;
            this.htmlInputValidator = htmlInputValidator;
            this.jobClient = jobClient;
        }

        public async Task<Project> CreateProject(NewProject newProject, ClaimsPrincipal user, CancellationToken token)
        {
            logger.LogInformation("Creating project: {0}", newProject.Name);
            ApplicationUser owner = await userManager.GetUserAsync(user);

            if (owner == null)
            {
                throw new ValidationException("User not found");
            }

            if (await context.Projects.AnyAsync(p => p.Name == newProject.Name))
            {
                throw new ValidationException("A project with this name already exists");
            }

            ValidateProject(newProject);

            var tagMap = new Dictionary<string, int>();
            if (newProject.Tags.Any())
            {
                var missingTags = newProject
                    .Tags
                    .Except(
                        await context.Tags
                                     .Where(t => newProject.Tags.Contains(t.Name))
                                     .Select(t => t.Name)
                                     .ToListAsync(token))
                    .Select(t => new Tag() { Name = t });

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags);
                    await context.SaveChangesAsync(token);
                }

                tagMap = await context.Tags
                                      .Where(t => newProject.Tags.Contains(t.Name))
                                      .ToDictionaryAsync(t => t.Name, t => t.Id, token);
            }

            List<ProjectTag> projectTags =
                newProject.Tags
                          .Select(t => new ProjectTag() { TagId = tagMap[t] })
                          .ToList();

            var project = new Project
            {
                OwnerId = owner.Id,
                Name = newProject.Name,
                Description = newProject.Description,
                Proposal = newProject.Proposal,
                Goal = newProject.Goal,
                CreatorComments = newProject.CreatorComments,
                Categories = newProject.Categories.Select(c => new ProjectCategory() { Category = c }).ToList(),
                Target = newProject.Target,
                Start = newProject.Start,
                Status = ProjectStatus.Hidden,
                End = newProject.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                BannerImageFileId = newProject.BannerImageFileId,
                DescriptiveImageFileId = newProject.DescriptiveImageFileId,
                DescriptionVideoLink = newProject.DescriptionVideoLink,
                Tags = projectTags
            };

            context.Projects.Add(project);
            await context.SaveChangesAsync(token);

            await RefreshParticipantCountMaterializedView(token);

            await emailSender.SendEmailTemplated(owner.Email, $"Thank you for submitting \"{project.Name}\" on CollAction", "ProjectConfirmation");

            IList<ApplicationUser> administrators = await userManager.GetUsersInRoleAsync(Constants.AdminRole);
            await emailSender.SendEmailsTemplated(administrators.Select(a => a.Email), $"A new project was submitted on CollAction: \"{project.Name}\"", "ProjectAddedAdmin", project.Name);

            logger.LogInformation("Created project: {0}", newProject.Name);

            return project;
        }

        public async Task<Project> UpdateProject(UpdatedProject updatedProject, ClaimsPrincipal user, CancellationToken token)
        {
            if (!user.IsInRole(Constants.AdminRole))
            {
                throw new ValidationException("Not allowed to update project");
            }

            logger.LogInformation("Updating project: {0}", updatedProject.Name);

            Project project = await context
                .Projects
                .Include(p => p.Tags).ThenInclude(t => t.Tag)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == updatedProject.Id, token);

            if (project == null)
            {
                throw new ValidationException("Project not found");
            }

            if (project.Name != updatedProject.Name && await context.Projects.AnyAsync(p => p.Name == updatedProject.Name))
            {
                throw new ValidationException("A project with this name already exists");
            }

            ValidateProject(updatedProject);

            bool approved = updatedProject.Status == ProjectStatus.Running && project.Status != ProjectStatus.Running;
            bool changeFinishJob = (approved || project.End != updatedProject.End) && updatedProject.End < DateTime.UtcNow;
            bool removeFinishJob = updatedProject.Status != ProjectStatus.Running && project.FinishJobId != null;
            bool deleted = updatedProject.Status == ProjectStatus.Deleted;

            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;
            project.Proposal = updatedProject.Proposal;
            project.Goal = updatedProject.Goal;
            project.CreatorComments = updatedProject.CreatorComments;
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

            var projectTags = project.Tags.Select(t => t.Tag.Name);
            if (!Enumerable.SequenceEqual(updatedProject.Tags.OrderBy(t => t), projectTags.OrderBy(t => t)))
            {
                IEnumerable<string> missingTags =
                    updatedProject.Tags
                                  .Except(
                                      await context.Tags
                                                   .Where(t => updatedProject.Tags.Contains(t.Name))
                                                   .Select(t => t.Name)
                                                   .ToListAsync(token));

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags.Select(t => new Tag() { Name = t }));
                    await context.SaveChangesAsync(token);
                }

                var tagMap = 
                    await context.Tags
                                 .Where(t => updatedProject.Tags.Contains(t.Name) || projectTags.Contains(t.Name))
                                 .ToDictionaryAsync(t => t.Name, t => t.Id, token);

                IEnumerable<ProjectTag> newTags =
                    updatedProject.Tags
                                  .Except(projectTags)
                                  .Select(t => new ProjectTag() { ProjectId = project.Id, TagId = tagMap[t] });
                IEnumerable<ProjectTag> removedTags =
                    project.Tags
                           .Where(t => projectTags.Except(updatedProject.Tags).Contains(t.Tag.Name));
                context.ProjectTags.AddRange(newTags);
                context.ProjectTags.RemoveRange(removedTags);
            }

            var categories = project.Categories.Select(c => c.Category);
            if (!Enumerable.SequenceEqual(categories.OrderBy(c => c), updatedProject.Categories.OrderBy(c => c)))
            {
                IEnumerable<Category> newCategories = updatedProject.Categories.Except(categories);
                IEnumerable<ProjectCategory> removedCategories = project.Categories.Where(pc => !updatedProject.Categories.Contains(pc.Category));

                context.ProjectCategories.RemoveRange(removedCategories);
                context.ProjectCategories.AddRange(newCategories.Select(c => new ProjectCategory() { ProjectId = project.Id, Category = c }));
            }

            ApplicationUser owner = await userManager.FindByIdAsync(project.OwnerId);

            if (approved)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Approval - {project.Name}", "ProjectApproval");
            }
            else if (deleted)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Deleted - {project.Name}", "ProjectDeleted");
            }

            if (changeFinishJob)
            {
                if (project.FinishJobId != null)
                {
                    jobClient.Delete(project.FinishJobId);
                }

                project.FinishJobId = jobClient.Schedule(() => ProjectSuccess(project.Id, CancellationToken.None), project.End);
            }
            else if (removeFinishJob)
            {
                jobClient.Delete(project.FinishJobId);
            }

            await context.SaveChangesAsync(token);
            logger.LogInformation("Updated project: {0}", updatedProject.Name);

            return project;
        }

        public async Task ProjectSuccess(int projectId, CancellationToken token)
        {
            Project project = await context.Projects.Include(p => p.ParticipantCounts).FirstAsync(p => p.Id == projectId, token);

            if (project.IsSuccessfull)
            {
                await emailSender.SendEmailTemplated(project.Owner.Email, $"Success - {project.Name}", "ProjectSuccess");
            }
            else if (project.IsFailed)
            {
                await emailSender.SendEmailTemplated(project.Owner.Email, $"Failed - {project.Name}", "ProjectFailed");
            }
        }

        public async Task<Project> SendProjectEmail(int projectId, string subject, string message, ClaimsPrincipal performingUser, CancellationToken token)
        {
            Project project = await context.Projects.FindAsync(new object[] { projectId }, token);
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
                             .ToListAsync(token);

            logger.LogInformation("sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());

            foreach (ProjectParticipant participant in participants)
            {
                string unsubscribeLink = $"{siteOptions.PublicAddress}/Manage/ChangeSubscription?userId={WebUtility.UrlEncode(participant.UserId)}&projectId={project.Id}&token={WebUtility.UrlEncode(participant.UnsubscribeToken.ToString())}";
                emailSender.SendEmail(participant.User.Email, subject, FormatEmailMessage(message, participant.User, unsubscribeLink));
            }

            IList<ApplicationUser> adminUsers = await userManager.GetUsersInRoleAsync(Constants.AdminRole);
            foreach (ApplicationUser admin in adminUsers)
            {
                emailSender.SendEmail(admin.Email, subject, FormatEmailMessage(message, admin, null));
            }

            emailSender.SendEmail(project.Owner.Email, subject, FormatEmailMessage(message, project.Owner, null));

            project.NumberProjectEmailsSend += 1;
            await context.SaveChangesAsync(token);

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
                   now >= project.Start;
        }

        public async Task<ProjectParticipant> SetEmailProjectSubscription(int projectId, string userId, Guid token, bool isSubscribed, CancellationToken cancellationToken)
        {
            ProjectParticipant participant = await context
                 .ProjectParticipants
                 .Include(p => p.Project)
                 .FirstAsync(p => p.ProjectId == projectId && p.UserId == userId);

            if (participant != null && participant.UnsubscribeToken == token)
            {
                logger.LogInformation("Setting project subscription for user");
                participant.SubscribedToProjectEmails = isSubscribed;
                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                logger.LogError("Unable to set project subscription for user, invalid token");
                throw new InvalidOperationException("Not authorized");
            }

            return participant;
        }

        public async Task<AddParticipantResult> CommitToProject(string email, int projectId, ClaimsPrincipal user, CancellationToken token)
        {
            ApplicationUser applicationUser = await userManager.GetUserAsync(user);
            if (applicationUser == null && string.IsNullOrEmpty(email))
            {
                return new AddParticipantResult()
                {
                    Error = "E-mail address not valid"
                };
            }

            Project project = await context.Projects.FindAsync(new object[] { projectId }, token);
            if (project == null)
            {
                return new AddParticipantResult()
                {
                    Error = "Project not found"
                };
            }

            logger.LogInformation("Adding participant to project");

            AddParticipantResult result = applicationUser != null
                ? await AddLoggedInParticipant(project, applicationUser, token)
                : await AddAnonymousParticipant(project, applicationUser, email, token);

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

                logger.LogInformation("Added participant to project");
            }
            else if (result.Error != null)
            {
                logger.LogError("Error adding participant to project: {0}", result.Error);
            }
            else
            {
                logger.LogInformation("Participation adding ended with: {0}", result.Scenario);
            }

            return result;
        }

        public IQueryable<Project> SearchProjects(Category? category, SearchProjectStatus? searchProjectStatus)
        {
            var projects = context.Projects.AsQueryable();

            switch (searchProjectStatus)
            {
                case SearchProjectStatus.Closed:
                    projects = projects.Where(p => p.End < DateTime.UtcNow && p.Status == ProjectStatus.Running);
                    break;
                case SearchProjectStatus.ComingSoon:
                    projects = projects.Where(p => p.Start > DateTime.UtcNow && p.Status == ProjectStatus.Running);
                    break;
                case SearchProjectStatus.Open:
                    projects = projects.Where(p => p.Start <= DateTime.UtcNow && p.End >= DateTime.UtcNow && p.Status == ProjectStatus.Running);
                    break;
            }

            if (category != null)
            {
                projects = projects.Where(p => p.Categories.Any(projectCategory => projectCategory.Category == category));
            }

            return projects;
        }

        private void ValidateProject(IProjectModel project)
        {
            if (project.Categories.Count > 2)
            {
                throw new ValidationException("Too many categories");
            }

            if (project.Categories.Count == 2 && project.Categories.First() == project.Categories.Last())
            {
                throw new ValidationException("Duplicate categories");
            }

            if (project.End < project.Start || project.Start < DateTime.UtcNow)
            {
                throw new ValidationException("Issue with project start and end dates");
            }

            if (project.DescriptionVideoLink != null && !Regex.IsMatch(project.DescriptionVideoLink, @"^https://www.youtube.com/watch\?v=[^& ]+$"))
            {
                throw new ValidationException("Project has invalid video link");
            }

            if (DateTime.UtcNow < project.Start.AddYears(-1))
            {
                throw new ValidationException("Please ensure your project starts within the next year");
            }

            if (project.End - project.Start > TimeSpan.FromDays(356))
            {
                throw new ValidationException("Please ensure your project end-date is within a year of the start-date");
            }
        }

        private async Task<AddParticipantResult> AddAnonymousParticipant(Project project, ApplicationUser user, string email, CancellationToken token)
        {
            var result = new AddParticipantResult();
            if (user == null)
            {
                user = new ApplicationUser() { Email = email, UserName = email, RepresentsNumberParticipants = 1, RegistrationDate = DateTime.UtcNow };
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

            result.UserAdded = await InsertParticipant(project.Id, user.Id, token);
            result.UserAlreadyActive = user.Activated;

            if (!result.UserAlreadyActive)
            {
                result.ParticipantEmail = user.Email;
                result.PasswordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            }

            return result;
        }

        private async Task<AddParticipantResult> AddLoggedInParticipant(Project project, ApplicationUser user, CancellationToken token)
        {
            bool added = await InsertParticipant(project.Id, user.Id, token);
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

        private Task RefreshParticipantCountMaterializedView(CancellationToken token)
            => context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY \"ProjectParticipantCounts\";", token);

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

        private async Task<bool> InsertParticipant(int projectId, string userId, CancellationToken token)
        {
            if (await context.ProjectParticipants.AnyAsync(part => part.UserId == userId && part.ProjectId == projectId))
            {
                return false;
            }

            ProjectParticipant participant = new ProjectParticipant
            {
                UserId = userId,
                ProjectId = projectId,
                SubscribedToProjectEmails = true,
                UnsubscribeToken = Guid.NewGuid(),
                ParticipationDate = DateTime.UtcNow
            };

            try
            {
                context.ProjectParticipants.Add(participant);

                await context.SaveChangesAsync(token);

                await RefreshParticipantCountMaterializedView(token);

                return true;
            }
            catch (DbUpdateException)
            {
                // User is already participating
                return false;
            }
        }
    }
}
