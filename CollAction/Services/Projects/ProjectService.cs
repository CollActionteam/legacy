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
using Microsoft.AspNetCore.Http;
using CollAction.Services.Image;
using System.IO;
using System.Net.Http;
using System.Net.Mail;

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
        private readonly IImageService imageService;

        public ProjectService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<ProjectService> logger,
            IOptions<ProjectEmailOptions> projectEmailOptions,
            IOptions<SiteOptions> siteOptions,
            IHtmlInputValidator htmlInputValidator,
            IServiceProvider serviceProvider,
            IBackgroundJobClient jobClient,
            IImageService imageService)
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
            this.imageService = imageService;
        }

        public async Task<ProjectResult> CreateProject(NewProject newProject, ClaimsPrincipal user, CancellationToken token)
        {
            logger.LogInformation("Validating new project");

            IEnumerable<ValidationResult> validationResults = ValidationHelper.Validate(newProject, serviceProvider);
            if (validationResults.Any())
            {
                return new ProjectResult(validationResults);
            }

            ApplicationUser owner = await userManager.GetUserAsync(user).ConfigureAwait(false);

            if (owner == null)
            {
                return new ProjectResult(new ValidationResult("Project owner could not be found"));
            }

            if (await context.Projects.AnyAsync(p => p.Name == newProject.Name).ConfigureAwait(false))
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
                                     .ToListAsync(token).ConfigureAwait(false))
                    .Select(t => new Tag(t));

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags);
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }

                tagMap = await context.Tags
                                      .Where(t => newProject.Tags.Contains(t.Name))
                                      .ToDictionaryAsync(t => t.Name, t => t.Id, token).ConfigureAwait(false);
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
                descriptionVideoLink: newProject.DescriptionVideoLink?.Replace("www.youtube.com", "www.youtube-nocookie.com", StringComparison.Ordinal),
                displayPriority: ProjectDisplayPriority.Medium, 
                bannerImageFileId: newProject.BannerImageFileId,
                descriptiveImageFileId: newProject.DescriptiveImageFileId,
                categories: newProject.Categories.Select(c => new ProjectCategory((c))).ToList(), 
                tags: projectTags);

            context.Projects.Add(project);
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            await RefreshParticipantCountMaterializedView(token).ConfigureAwait(false);

            await emailSender.SendEmailTemplated(owner.Email, $"Thank you for submitting \"{project.Name}\" on CollAction", "ProjectConfirmation").ConfigureAwait(false);

            IList<ApplicationUser> administrators = await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole).ConfigureAwait(false);
            await emailSender.SendEmailsTemplated(administrators.Select(a => a.Email), $"A new project was submitted on CollAction: \"{project.Name}\"", "ProjectAddedAdmin", project.Name).ConfigureAwait(false);

            logger.LogInformation("Created project: {0}", newProject.Name);

            return new ProjectResult(project);
        }

        public async Task<int> DeleteProject(int id, CancellationToken token)
        {
            var project = await context.Projects.FindAsync(id);

            if (project == null)
            {
                throw new InvalidOperationException($"Project {id} doesn't exist");
            }

            project.Status = ProjectStatus.Deleted;
            await context.SaveChangesAsync().ConfigureAwait(false);

            return id;
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
                .FirstOrDefaultAsync(p => p.Id == updatedProject.Id, token).ConfigureAwait(false);

            if (project == null)
            {
                return new ProjectResult(new ValidationResult("Project not found", new[] { nameof(Project.Id) }));
            }

            if (project.Name != updatedProject.Name && await context.Projects.AnyAsync(p => p.Name == updatedProject.Name).ConfigureAwait(false))
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
            project.DescriptionVideoLink = updatedProject.DescriptionVideoLink?.Replace("www.youtube.com", "www.youtube-nocookie.com", StringComparison.Ordinal);
            project.Status = updatedProject.Status;
            project.DisplayPriority = updatedProject.DisplayPriority;
            project.NumberProjectEmailsSent = updatedProject.NumberProjectEmailsSent;
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
                                                   .ToListAsync(token).ConfigureAwait(false));

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags.Select(t => new Tag(t)));
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }

                var tagMap = 
                    await context.Tags
                                 .Where(t => updatedProject.Tags.Contains(t.Name) || projectTags.Contains(t.Name))
                                 .ToDictionaryAsync(t => t.Name, t => t.Id, token).ConfigureAwait(false);

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

            ApplicationUser owner = await userManager.FindByIdAsync(project.OwnerId).ConfigureAwait(false);

            if (approved)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Approval - {project.Name}", "ProjectApproval").ConfigureAwait(false);
            }
            else if (deleted)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Deleted - {project.Name}", "ProjectDeleted").ConfigureAwait(false);
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

            await context.SaveChangesAsync(token).ConfigureAwait(false);
            logger.LogInformation("Updated project: {0}", updatedProject.Name);

            return new ProjectResult(project);
        }

        public async Task ProjectSuccess(int projectId, CancellationToken token)
        {
            Project project = await context.Projects.Include(p => p.ParticipantCounts).FirstAsync(p => p.Id == projectId, token).ConfigureAwait(false);

            if (project.IsSuccessfull && project.Owner != null)
            {
                await emailSender.SendEmailTemplated(project.Owner.Email, $"Success - {project.Name}", "ProjectSuccess").ConfigureAwait(false);
            }
            else if (project.IsFailed && project.Owner != null)
            {
                await emailSender.SendEmailTemplated(project.Owner.Email, $"Failed - {project.Name}", "ProjectFailed").ConfigureAwait(false);
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

            ApplicationUser user = await userManager.GetUserAsync(performingUser).ConfigureAwait(false);
            if (project.OwnerId != user.Id)
            {
                return new ProjectResult(new ValidationResult("Unauthorized"));
            }

            IEnumerable<ProjectParticipant> participants =
                await context.ProjectParticipants
                             .Include(part => part.User)
                             .Where(part => part.ProjectId == project.Id && part.SubscribedToProjectEmails)
                             .ToListAsync(token).ConfigureAwait(false);

            logger.LogInformation("sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());

            foreach (ProjectParticipant participant in participants)
            {
                string unsubscribeLink = $"{siteOptions.CanonicalAddress}{project.Url}/unsubscribe-email?userId={WebUtility.UrlEncode(participant.UserId)}&token={WebUtility.UrlEncode(participant.UnsubscribeToken.ToString())}";
                emailSender.SendEmail(participant.User.Email, subject, FormatEmailMessage(message, participant.User, unsubscribeLink));
            }

            IList<ApplicationUser> adminUsers = await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole).ConfigureAwait(false);
            foreach (ApplicationUser admin in adminUsers)
            {
                emailSender.SendEmail(admin.Email, subject, FormatEmailMessage(message, admin, null));
            }

            if (project.Owner != null)
            {
                emailSender.SendEmail(project.Owner.Email, subject, FormatEmailMessage(message, project.Owner, null));
            }

            project.NumberProjectEmailsSent += 1;
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            logger.LogInformation("done sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());
            return new ProjectResult(project);
        }

        public bool CanSendProjectEmail(Project project)
        {
            DateTime now = DateTime.UtcNow;
            return project.NumberProjectEmailsSent < projectEmailOptions.MaxNumberProjectEmails &&
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
                 .FirstAsync(p => p.ProjectId == projectId && p.UserId == userId).ConfigureAwait(false);

            if (participant != null && participant.UnsubscribeToken == token)
            {
                logger.LogInformation("Setting project subscription for user");
                participant.SubscribedToProjectEmails = isSubscribed;
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                logger.LogError("Unable to set project subscription for user, invalid token");
                throw new InvalidOperationException("Not authorized");
            }

            return participant;
        }

        public async Task<AddParticipantResult> CommitToProjectAnonymous(string email, int projectId, CancellationToken token)
        {
            Project project = await context.Projects.FindAsync(new object[] { projectId }, token);
            if (project == null || !project.IsActive)
            {
                logger.LogError("Project not found or active: {0}", projectId);
                return new AddParticipantResult($"Project not found or is not active");
            }

            if (!IsValidEmail(email))
            {
                logger.LogWarning("Invalid e-mail signing up to project");
                return new AddParticipantResult("Invalid e-mail address");
            }

            var result = new AddParticipantResult();
            ApplicationUser? user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            {
                user = new ApplicationUser(email: email, registrationDate: DateTime.UtcNow);
                IdentityResult creationResult = await userManager.CreateAsync(user).ConfigureAwait(false);
                if (!creationResult.Succeeded)
                {
                    string errors = string.Join(',', creationResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    logger.LogError("Could not create new unregistered user for project commit: {0}", errors);
                    return new AddParticipantResult($"Could not create new unregistered user: {errors}");
                }
                result.UserCreated = true;
            }

            result.UserAdded = await InsertParticipant(project.Id, user.Id, token).ConfigureAwait(false);
            result.UserAlreadyActive = user.Activated;

            logger.LogInformation("Added participant to project: {0}, {1}", user.Id, projectId);

            if (!user.Activated)
            {
                result.ParticipantEmail = user.Email;
                result.PasswordResetToken = await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            }

            if (result.Scenario != AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating &&
                result.Scenario != AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating)
            {
                await SendCommitEmail(project, result, user, user.Email).ConfigureAwait(false);
            }

            logger.LogInformation("Added participant '{0}' to project '{1}' with scenario '{2}'", user.Id, projectId, result.Scenario);

            return result;
        }

        public async Task<AddParticipantResult> CommitToProjectLoggedIn(ClaimsPrincipal user, int projectId, CancellationToken token)
        {
            Project project = await context.Projects.FindAsync(new object[] { projectId }, token);
            if (project == null || !project.IsActive)
            {
                logger.LogError("Project not found or active: {0}", projectId);
                return new AddParticipantResult("Project not found or not active");
            }

            ApplicationUser applicationUser = await userManager.GetUserAsync(user).ConfigureAwait(false);
            if (applicationUser == null)
            {
                logger.LogError("User not logged in when committing");
                return new AddParticipantResult(error: "User not logged in");
            }

            bool added = await InsertParticipant(project.Id, applicationUser.Id, token).ConfigureAwait(false);
            var result = new AddParticipantResult(loggedIn: true, userAdded: added);

            if (added)
            {
                await SendCommitEmail(project, result, applicationUser, applicationUser.Email).ConfigureAwait(false);
            }

            logger.LogInformation("Added participant '{0}' to project '{1}' with scenario '{2}'", applicationUser.Id, projectId, result.Scenario);

            return result;
        }

        public IQueryable<Project> SearchProjects(Category? category, SearchProjectStatus? searchProjectStatus)
        {
            IQueryable<Project> projects = context
                .Projects
                .Include(p => p.ParticipantCounts)
                .OrderBy(p => p.DisplayPriority)
                .AsQueryable();

            projects = searchProjectStatus switch
            {
                SearchProjectStatus.Closed => projects.Where(p => p.End < DateTime.UtcNow && p.Status == ProjectStatus.Running),
                SearchProjectStatus.ComingSoon => projects.Where(p => p.Start > DateTime.UtcNow && p.Status == ProjectStatus.Running),
                SearchProjectStatus.Open => projects.Where(p => p.Start <= DateTime.UtcNow && p.End >= DateTime.UtcNow && p.Status == ProjectStatus.Running),
                SearchProjectStatus.Active => projects.Where(p => p.Status == ProjectStatus.Running),
                _ => projects.Where(p => p.Status != ProjectStatus.Deleted)
            };

            if (category != null)
            {
                projects = projects.Include(p => p.Categories).Where(p => p.Categories.Any(projectCategory => projectCategory.Category == category));
            }

            return projects;
        }

        public async Task SeedRandomProjects(IEnumerable<ApplicationUser> users, CancellationToken cancellationToken)
        {
            Random r = new Random();

            string?[] videoLinks = new[]
            {
                "https://www.youtube-nocookie.com/embed/aLzM_L5fjCQ",
                "https://www.youtube-nocookie.com/embed/Zvugem-tKyI",
                "https://www.youtube-nocookie.com/embed/xY0XTysJUDY",
                "https://www.youtube-nocookie.com/embed/2yfPLxQQG-k",
                null
            };

            List<(Uri bannerImageUrl, Task<byte[]> bannerImageBytes)> bannerImages = new[]
            {
                "https://collaction-production.s3.eu-central-1.amazonaws.com/57136ed4-b7f6-4dd2-a822-9341e2e60d1e.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/765bc57b-748e-4bb8-a27e-08db6b99ea3e.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/e06bbc2d-02f7-4a9b-a744-6923d5b21f51.png",
            }.Select(b => new Uri(b)).Select(b => (b, DownloadFile(b, cancellationToken))).ToList();

            List<(Uri descriptiveImageUrl, Task<byte[]> descriptiveImageBytes)> descriptiveImages = new[]
            {
                "https://collaction-production.s3.eu-central-1.amazonaws.com/107104bc-deeb-4f48-b3a5-f25585bebf89.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/365f2dc9-1784-45ea-9cc7-d5f0ef1a480c.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/6e6c12b1-eaae-4811-aa1c-c169d10f1a59.png",
            }.Select(b => new Uri(b)).Select(b => (b, DownloadFile(b, cancellationToken))).ToList();

            await Task.WhenAll(descriptiveImages.Select(d => d.descriptiveImageBytes).Concat(bannerImages.Select(b => b.bannerImageBytes))).ConfigureAwait(false);

            List<Tag> tags = Enumerable.Range(10, r.Next(60))
                                       .Select(r => Faker.Internet.DomainWord())
                                       .Distinct()
                                       .Select(r => new Tag(r))
                                       .ToList();
            context.Tags.AddRange(tags);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var projectNames =
                Enumerable.Range(0, r.Next(40, 80))
                          .Select(i => Faker.Company.Name())
                          .Distinct();

            List<string> userIds = await context.Users.Select(u => u.Id).ToListAsync().ConfigureAwait(false);

            // Generate random projects
            foreach (string projectName in projectNames)
            {
                DateTime start = DateTime.Now.Date.AddDays(r.Next(-20, 20));

                List<ProjectTag> projectTags =
                    Enumerable.Range(0, r.Next(5))
                              .Select(i => r.Next(tags.Count))
                              .Distinct()
                              .Select(i => new ProjectTag(tags.ElementAt(i).Id))
                              .ToList();

                List<ProjectCategory> categories =
                    new[] { r.Next(7), r.Next(7) }.Distinct()
                                                  .Select(i => new ProjectCategory((Category)i))
                                                  .ToList();

                (Uri descriptiveImageUrl, Task<byte[]> descriptiveImageBytes) = descriptiveImages[r.Next(descriptiveImages.Count)];
                ImageFile? descriptiveImage = r.Next(3) == 0
                                                  ? null
                                                  : await imageService.UploadImage(ToFormFile(descriptiveImageBytes.Result, descriptiveImageUrl), Faker.Company.BS(), cancellationToken).ConfigureAwait(false);

                (Uri bannerImageUrl, Task<byte[]> bannerImageBytes) = bannerImages[r.Next(bannerImages.Count)];
                ImageFile? bannerImage = r.Next(3) == 0
                                             ? null
                                             : await imageService.UploadImage(ToFormFile(bannerImageBytes.Result, bannerImageUrl), Faker.Company.BS(), cancellationToken).ConfigureAwait(false);

                List<ProjectParticipant> projectParticipants = userIds
                        .Where(userId => r.Next(3) == 0)
                        .Select(userId => new ProjectParticipant(userId, 0, r.Next(2) == 1, DateTime.UtcNow, Guid.NewGuid()))
                        .ToList();

                context.Projects.Add(
                    new Project(
                        name: projectName,
                        description: $"<p>{string.Join("</p><p>", Faker.Lorem.Paragraphs(r.Next(3) + 1))}</p>",
                        start: start,
                        end: start.AddDays(r.Next(10, 40)).AddHours(23).AddMinutes(59).AddSeconds(59),
                        categories: categories,
                        tags: projectTags,
                        bannerImageFileId: bannerImage?.Id,
                        descriptiveImageFileId: descriptiveImage?.Id,
                        creatorComments: r.Next(4) == 0 ? null : $"<p>{string.Join("</p><p>", Faker.Lorem.Paragraphs(r.Next(3) + 1))}</p>",
                        displayPriority: (ProjectDisplayPriority)r.Next(0, 2),
                        goal: Faker.Company.CatchPhrase(),
                        ownerId: r.Next(10) == 0 ? null : users.ElementAt(r.Next(users.Count())).Id,
                        proposal: Faker.Company.BS(),
                        status: (ProjectStatus)r.Next(0, 3),
                        target: r.Next(1, 10000),
                        anonymousUserParticipants: r.Next(1, 8000),
                        descriptionVideoLink: videoLinks.ElementAt(r.Next(videoLinks.Length))) { Participants = projectParticipants });
            }
            
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await RefreshParticipantCountMaterializedView(cancellationToken).ConfigureAwait(false);
        }

        private IFormFile ToFormFile(byte[] imageBytes, Uri url)
        {
            return new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, url.LocalPath, url.LocalPath);
        }

        private async Task<byte[]> DownloadFile(Uri url, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        private async Task SendCommitEmail(Project project, AddParticipantResult result, ApplicationUser applicationUser, string email)
        {
            var commitEmailViewModel = new ProjectCommitEmailViewModel(project: project, result: result, user: applicationUser, publicAddress: new Uri(siteOptions.CanonicalAddress), projectUrl: new Uri($"https://{siteOptions.PublicAddress}/{project.Url}"));
            await emailSender.SendEmailTemplated(email, $"Thank you for participating in the \"{project.Name}\" project on CollAction", "ProjectCommit", commitEmailViewModel).ConfigureAwait(false);
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
                message = message.Replace(" {firstname}", string.Empty, StringComparison.Ordinal)
                                 .Replace("{firstname}", string.Empty, StringComparison.Ordinal);
            }
            else
            {
                message = message.Replace("{firstname}", user.FirstName, StringComparison.Ordinal);
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                message = message.Replace(" {lastname}", string.Empty, StringComparison.Ordinal)
                                 .Replace("{lastname}", string.Empty, StringComparison.Ordinal);
            }
            else
            {
                message = message.Replace("{lastname}", user.LastName, StringComparison.Ordinal);
            }

            return message;
        }

        private async Task<bool> InsertParticipant(int projectId, string userId, CancellationToken token)
        {
            logger.LogInformation("Adding participant '{0}' to project '{1}'", userId, projectId);

            if (await context.ProjectParticipants.AnyAsync(part => part.UserId == userId && part.ProjectId == projectId).ConfigureAwait(false))
            {
                return false;
            }

            ProjectParticipant participant = new ProjectParticipant(userId: userId, projectId: projectId, subscribedToProjectEmails: true, participationDate: DateTime.UtcNow, unsubscribeToken: Guid.NewGuid());

            try
            {
                context.ProjectParticipants.Add(participant);

                await context.SaveChangesAsync(token).ConfigureAwait(false);

                await RefreshParticipantCountMaterializedView(token).ConfigureAwait(false);

                logger.LogInformation("Added participant '{0}' to project '{1}'", userId, projectId);

                return true;
            }
            catch (DbUpdateException e)
            {
                logger.LogWarning(e, "Duplicate project subscription, failure adding participant '{0}' to project '{1}'", userId, projectId);
                // User is already participating
                return false;
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
