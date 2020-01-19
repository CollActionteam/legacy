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
using CollAction.Helpers;

namespace CollAction.Services.Projects
{
    public sealed class ProjectService : IProjectService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly IEmailSender emailSender;
        private readonly ILogger<ProjectService> logger;
        private readonly ProjectEmailOptions projectEmailOptions;
        private readonly SiteOptions siteOptions;
        private readonly IHtmlInputValidator htmlInputValidator;
        private readonly IServiceProvider serviceProvider;
        private readonly IBackgroundJobClient jobClient;

        public ProjectService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<ProjectService> logger,
            IOptions<ProjectEmailOptions> projectEmailOptions,
            IOptions<SiteOptions> siteOptions,
            IHtmlInputValidator htmlInputValidator,
            IServiceProvider serviceProvider,
            IBackgroundJobClient jobClient)
        {
            this.userManager = userManager;
            this.context = context;
            this.emailSender = emailSender;
            this.logger = logger;
            this.projectEmailOptions = projectEmailOptions.Value;
            this.siteOptions = siteOptions.Value;
            this.htmlInputValidator = htmlInputValidator;
            this.serviceProvider = serviceProvider;
            this.jobClient = jobClient;
        }

        public async Task<ProjectResult> CreateProject(NewProject newProject, ClaimsPrincipal user, CancellationToken token)
        {
            logger.LogInformation("Validating new project");

            IEnumerable<ValidationResult> validationResults = ValidationHelper.Validate(newProject, serviceProvider);
            if (validationResults.Any())
            {
                return new ProjectResult(validationResults);
            }

            ApplicationUser owner = await userManager.GetUserAsync(user);

            if (owner == null)
            {
                return new ProjectResult(new ValidationResult("Project owner could not be found"));
            }

            if (await context.Projects.AnyAsync(p => p.Name == newProject.Name))
            {
                return new ProjectResult(new ValidationResult("A project with this name already exists", new[] { nameof(Project.Name) }));
            }

            logger.LogInformation("Creating project: {0}", newProject.Name);
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
                    .Select(t => new Tag(t));

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
                          .Select(t => new ProjectTag(tagId: tagMap[t]))
                          .ToList();

            var project = new Project(
                name: newProject.Name,
                status: ProjectStatus.Hidden,
                ownerId: owner.Id,
                target: newProject.Target,
                start: newProject.Start,
                end: newProject.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                description: newProject.Description,
                goal: newProject.Goal, 
                proposal: newProject.Proposal,
                creatorComments: newProject.CreatorComments,
                descriptionVideoLink: newProject.DescriptionVideoLink,
                displayPriority: ProjectDisplayPriority.Medium, 
                bannerImageFileId: newProject.BannerImageFileId,
                descriptiveImageFileId: newProject.DescriptiveImageFileId,
                categories: newProject.Categories.Select(c => new ProjectCategory((c))).ToList(), 
                tags: projectTags);

            context.Projects.Add(project);
            await context.SaveChangesAsync(token);

            await RefreshParticipantCountMaterializedView(token);

            await emailSender.SendEmailTemplated(owner.Email, $"Thank you for submitting \"{project.Name}\" on CollAction", "ProjectConfirmation");

            IList<ApplicationUser> administrators = await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole);
            await emailSender.SendEmailsTemplated(administrators.Select(a => a.Email), $"A new project was submitted on CollAction: \"{project.Name}\"", "ProjectAddedAdmin", project.Name);

            logger.LogInformation("Created project: {0}", newProject.Name);

            return new ProjectResult(project);
        }

        public async Task<ProjectResult> UpdateProject(UpdatedProject updatedProject, ClaimsPrincipal user, CancellationToken token)
        {
            logger.LogInformation("Validating updated project");

            IEnumerable<ValidationResult> validationResults = ValidationHelper.Validate(updatedProject, serviceProvider);
            if (validationResults.Any())
            {
                return new ProjectResult(validationResults);
            }

            if (!user.IsInRole(AuthorizationConstants.AdminRole))
            {
                return new ProjectResult(new ValidationResult("User is not allowed to update project"));
            }

            Project project = await context
                .Projects
                .Include(p => p.Tags).ThenInclude(t => t.Tag)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == updatedProject.Id, token);

            if (project == null)
            {
                return new ProjectResult(new ValidationResult("Project not found", new[] { nameof(Project.Id) }));
            }

            if (project.Name != updatedProject.Name && await context.Projects.AnyAsync(p => p.Name == updatedProject.Name))
            {
                return new ProjectResult(new ValidationResult("A project with this name already exists", new[] { nameof(Project.Name) }));
            }

            logger.LogInformation("Updating project: {0}", updatedProject.Name);

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
            project.OwnerId = updatedProject.OwnerId;
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
                    context.Tags.AddRange(missingTags.Select(t => new Tag(t)));
                    await context.SaveChangesAsync(token);
                }

                var tagMap = 
                    await context.Tags
                                 .Where(t => updatedProject.Tags.Contains(t.Name) || projectTags.Contains(t.Name))
                                 .ToDictionaryAsync(t => t.Name, t => t.Id, token);

                IEnumerable<ProjectTag> newTags =
                    updatedProject.Tags
                                  .Except(projectTags)
                                  .Select(t => new ProjectTag(projectId: project.Id, tagId: tagMap[t]));
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
                context.ProjectCategories.AddRange(newCategories.Select(c => new ProjectCategory(projectId: project.Id, category: c)));
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

            return new ProjectResult(project);
        }

        public async Task ProjectSuccess(int projectId, CancellationToken token)
        {
            Project project = await context.Projects.Include(p => p.ParticipantCounts).FirstAsync(p => p.Id == projectId, token);

            if (project.IsSuccessfull && project.Owner != null)
            {
                await emailSender.SendEmailTemplated(project.Owner.Email, $"Success - {project.Name}", "ProjectSuccess");
            }
            else if (project.IsFailed && project.Owner != null)
            {
                await emailSender.SendEmailTemplated(project.Owner.Email, $"Failed - {project.Name}", "ProjectFailed");
            }
        }

        public async Task<ProjectResult> SendProjectEmail(int projectId, string subject, string message, ClaimsPrincipal performingUser, CancellationToken token)
        {
            Project project = await context.Projects.FindAsync(new object[] { projectId }, token);
            if (project == null)
            {
                return new ProjectResult(new ValidationResult("Project could not be found", new[] { nameof(projectId) }));
            }

            if (!(htmlInputValidator.IsSafe(message) && htmlInputValidator.IsSafe(subject)))
            {
                return new ProjectResult(new ValidationResult("Unsafe HTML in e-mail message", new[] { nameof(message) }));
            }

            ApplicationUser user = await userManager.GetUserAsync(performingUser);
            if (project.OwnerId != user.Id)
            {
                return new ProjectResult(new ValidationResult("Unauthorized"));
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

            IList<ApplicationUser> adminUsers = await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole);
            foreach (ApplicationUser admin in adminUsers)
            {
                emailSender.SendEmail(admin.Email, subject, FormatEmailMessage(message, admin, null));
            }

            if (project.Owner != null)
            {
                emailSender.SendEmail(project.Owner.Email, subject, FormatEmailMessage(message, project.Owner, null));
            }

            project.NumberProjectEmailsSend += 1;
            await context.SaveChangesAsync(token);

            logger.LogInformation("done sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());
            return new ProjectResult(project);
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
                return new AddParticipantResult(error: "E-mail address not valid");
            }

            Project project = await context.Projects.FindAsync(new object[] { projectId }, token);
            if (project == null)
            {
                return new AddParticipantResult("Project not found");
            }

            logger.LogInformation("Adding participant to project");

            AddParticipantResult result = applicationUser != null
                ? await AddLoggedInParticipant(project, applicationUser, token)
                : await AddAnonymousParticipant(project, email, token);

            if (result.Scenario != AddParticipantScenario.Error &&
                result.Scenario != AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating &&
                result.Scenario != AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating)
            {
                var commitEmailViewModel = new ProjectCommitEmailViewModel(project: project, result: result, user: applicationUser, publicAddress: siteOptions.PublicAddress, projectUrl: $"https://{siteOptions.PublicAddress}/{project.Url}");
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
            var projects = context.Projects.Include(p => p.ParticipantCounts).OrderBy(p => p.DisplayPriority).AsQueryable();

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
                projects = projects.Include(p => p.Categories).Where(p => p.Categories.Any(projectCategory => projectCategory.Category == category));
            }

            return projects;
        }

        public async Task SeedRandomProjects(ApplicationUser owningUser, CancellationToken cancellationToken)
        {
            Random r = new Random();
            context.Projects.AddRange(
                Enumerable.Range(0, r.Next(20, 200))
                          .Select(i =>
                          {
                              DateTime start = DateTime.Now.Date.AddDays(r.Next(-20, 20));

                              return new Project(
                                  name: Faker.Company.Name(),
                                  description: $"<p>{string.Join("</p><p>", Faker.Lorem.Paragraphs(r.Next(3) + 1))}</p>",
                                  start: start,
                                  end: start.AddDays(r.Next(10, 40)).AddHours(23).AddMinutes(59).AddSeconds(59),
                                  categories: new[] { r.Next(7), r.Next(7) }.Distinct().Select(i => new ProjectCategory((Category)i)).ToList(),
                                  tags: new List<ProjectTag>(),
                                  creatorComments: $"<p>{string.Join("</p><p>", Faker.Lorem.Paragraphs(r.Next(3) + 1))}</p>",
                                  displayPriority: (ProjectDisplayPriority)r.Next(0, 2),
                                  goal: Faker.Company.CatchPhrase(),
                                  ownerId: owningUser.Id,
                                  proposal: Faker.Company.BS(),
                                  status: (ProjectStatus)r.Next(0, 3),
                                  target: r.Next(1, 10000),
                                  anonymousUserParticipants: r.Next(1, 10000),
                                  descriptionVideoLink: $"https://www.youtube.com/watch?v={Guid.NewGuid()}");
                          }).Distinct(new LambdaEqualityComparer<Project, string>(p => p?.Name)));
            await context.SaveChangesAsync(cancellationToken);
            await RefreshParticipantCountMaterializedView(cancellationToken);
        }

        private async Task<AddParticipantResult> AddAnonymousParticipant(Project project, string email, CancellationToken token)
        {
            var user = new ApplicationUser(email: email, registrationDate: DateTime.UtcNow);
            IdentityResult creationResult = await userManager.CreateAsync(user);
            if (!creationResult.Succeeded)
            {
                string errors = string.Join(',', creationResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                return new AddParticipantResult($"Could not create new unregistered user {email}: {errors}");
            }

            var userAdded = await InsertParticipant(project.Id, user.Id, token);

            if (!user.Activated)
            {
                return new AddParticipantResult(userCreated: true, userAdded: userAdded, userAlreadyActive: user.Activated, participantEmail: user.Email, passwordResetToken: await userManager.GeneratePasswordResetTokenAsync(user));
            }
            else
            {
                return new AddParticipantResult(userCreated: true, userAdded: userAdded, userAlreadyActive: user.Activated);
            }
        }

        private async Task<AddParticipantResult> AddLoggedInParticipant(Project project, ApplicationUser user, CancellationToken token)
        {
            bool added = await InsertParticipant(project.Id, user.Id, token);
            if (!added)
            {
                // This is not a valid scenario
                return new AddParticipantResult($"User {user.Id} is already participating in project {project.Name}.");
            }

            return new AddParticipantResult(loggedIn: true, userAdded: true);
        }

        private Task RefreshParticipantCountMaterializedView(CancellationToken token)
            => context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY \"ProjectParticipantCounts\";", token);

        private string FormatEmailMessage(string message, ApplicationUser user, string? unsubscribeLink)
        {
            if (!string.IsNullOrEmpty(unsubscribeLink))
            {
                message += $"<br><a href=\"{unsubscribeLink}\">Unsubscribe</a>";
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

            ProjectParticipant participant = new ProjectParticipant(userId: userId, projectId: projectId, subscribedToProjectEmails: true, participationDate: DateTime.UtcNow, unsubscribeToken: Guid.NewGuid());

            try
            {
                context.ProjectParticipants.Add(participant);

                await context.SaveChangesAsync(token);

                await RefreshParticipantCountMaterializedView(token);

                return true;
            }
            catch (DbUpdateException e)
            {
                logger.LogWarning(e, "Duplicate project subscription");
                // User is already participating
                return false;
            }
        }
    }
}
