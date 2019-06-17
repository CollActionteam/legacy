using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Email;
using CollAction.Services.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using CollAction.Models.ProjectViewModels;

namespace CollAction.Services.Project
{
    public class ProjectService : IProjectService
    {
        private readonly IImageService _imageService;
        private readonly ApplicationDbContext _context;
        private readonly ProjectEmailOptions _projectEmailOptions;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ProjectService> _logger;
        private static Regex _spaceRemoveRegex = new Regex(@"\s", RegexOptions.Compiled);
        private static Regex _invalidCharRemoveRegex = new Regex(@"[^a-z0-9\s-_]", RegexOptions.Compiled);
        private static Regex _doubleDashRemoveRegex = new Regex(@"([-_]){2,}", RegexOptions.Compiled);

        public ProjectService(ApplicationDbContext context, IEmailSender emailSender, IImageService imageService, IOptions<ProjectEmailOptions> projectEmailOptions, ILogger<ProjectService> logger, UserManager<ApplicationUser> userManager)
        {
            _imageService = imageService;
            _context = context;
            _projectEmailOptions = projectEmailOptions.Value;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        public Task<Models.Project> GetProjectById(int id)
            => _context.Projects.SingleOrDefaultAsync(p => p.Id == id);

        public IQueryable<DisplayProjectViewModel> GetProjectDisplayViewModels(Expression<Func<Models.Project, bool>> filter)
        {
            return _context.Projects
                .Where(filter)
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.BannerImage)
                .Include(p => p.DescriptiveImage)
                .Include(p => p.DescriptionVideoLink)
                .Include(p => p.Owner)
                .Include(p => p.Tags).ThenInclude(t => t.Tag)
                .Include(p => p.ParticipantCounts)
                .Select(project =>
                            new DisplayProjectViewModel
                            {
                                Project = project,
                                Participants = project.ParticipantCounts.Count,
                                BannerImagePath = project.BannerImage == null ? $"/images/default_banners/{project.Category.Name}.jpg" : _imageService.GetUrl(project.BannerImage),
                                DescriptiveImagePath = project.DescriptiveImage == null ? null : _imageService.GetUrl(project.DescriptiveImage)
                            });
        }

        public bool CanSendProjectEmail(Models.Project project)
        {
            DateTime now = DateTime.UtcNow;
            return project.NumberProjectEmailsSend < _projectEmailOptions.MaxNumberProjectEmails &&
                   project.End + _projectEmailOptions.TimeEmailAllowedAfterProjectEnd > now &&
                   project.Status != ProjectStatus.Deleted &&
                   project.Status != ProjectStatus.Hidden &&
                   project.Status != ProjectStatus.Failed &&
                   now >= project.Start;
        }

        public int NumberEmailsAllowedToSend(Models.Project project)
            => _projectEmailOptions.MaxNumberProjectEmails - project.NumberProjectEmailsSend;

        public DateTime CanSendEmailsUntil(Models.Project project)
            => project.End + _projectEmailOptions.TimeEmailAllowedAfterProjectEnd;

        public async Task SendProjectEmail(Models.Project project, string subject, string message, HttpRequest request, IUrlHelper helper)
        {
            if (CanSendProjectEmail(project))
            {
                IEnumerable<ProjectParticipant> participants =
                    await _context.ProjectParticipants
                                  .Include(part => part.User)
                                  .Where(part => part.ProjectId == project.Id && part.SubscribedToProjectEmails)
                                  .ToListAsync();

                _logger.LogInformation("sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());

                string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
                foreach (ProjectParticipant participant in participants)
                {
                    string unsubscribeLink = $"{baseUrl}{helper.Action("ChangeSubscriptionFromToken", "Projects", new { UserId = participant.UserId, ProjectId = project.Id, UnsubscribeToken = participant.UnsubscribeToken })}";
                    _emailSender.SendEmail(participant.User.Email, subject, FormatEmailMessage(message, participant.User, unsubscribeLink));
                }

                IList<ApplicationUser> adminUsers = await _userManager.GetUsersInRoleAsync(Constants.AdminRole);
                foreach (ApplicationUser admin in adminUsers)
                {
                    _emailSender.SendEmail(admin.Email, subject, FormatEmailMessage(message, admin, null));
                }

                _emailSender.SendEmail(project.Owner.Email, subject, FormatEmailMessage(message, project.Owner, null));

                project.NumberProjectEmailsSend += 1;
                await _context.SaveChangesAsync();

                _logger.LogInformation("done sending project email for '{0}' on {1} to {2} users", project.Name, subject, participants.Count());
            }
            else
                throw new InvalidOperationException("unable to send project e-mails, limit exceeded");
        }

        private string FormatEmailMessage(string message, ApplicationUser user, string unsubscribeLink)
        {
            if (!string.IsNullOrEmpty(unsubscribeLink))
                message = message + $"<br><a href=\"{unsubscribeLink}\">Unsubscribe</a>";

            if (string.IsNullOrEmpty(user.FirstName))
                message = message.Replace(" {firstname}", string.Empty)
                                 .Replace("{firstname}", string.Empty);
            else
                message = message.Replace("{firstname}", user.FirstName);

            if (string.IsNullOrEmpty(user.LastName))
                message = message.Replace(" {lastname}", string.Empty)
                                 .Replace("{lastname}", string.Empty);
            else
                message = message.Replace("{lastname}", user.LastName);

            return message;
        }

        private static string ToUrlSlug(string value)
        {
            value = value.ToLowerInvariant();
            value = _spaceRemoveRegex.Replace(value, "-");
            value = _invalidCharRemoveRegex.Replace(value, "");
            value = value.Trim('-', '_');
            value = _doubleDashRemoveRegex.Replace(value, "$1");
            if (value.Length == 0)
            {
                value = "-";
            }

            return value;
        }

        private static string RemoveDiacriticsFromString(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public string GetProjectNameNormalized(string projectName)
        {
            projectName = RemoveDiacriticsFromString(projectName);
            projectName = ToUrlSlug(projectName);
            return projectName;
        }

        public async Task<FindProjectsViewModel> FindProject(int projectId)
        {
            var project =  await _context
                .Projects
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.BannerImage)
                .Include(p => p.ParticipantCounts)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            return project != null
                ? CreateFindProjectsViewModel(project)
                : null;
        }

        public IQueryable<FindProjectsViewModel> FindProjects(Expression<Func<Models.Project, bool>> filter, int? limit, int? start)
        {
            IQueryable<Models.Project> projects = _context
                .Projects
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.BannerImage)
                .Include(p => p.ParticipantCounts)
                .Where(filter)
                .OrderBy(p => p.DisplayPriority);

            if (limit.HasValue)
                projects = projects.Take(limit.Value);

            if(start.HasValue)
                projects = projects.Skip(start.Value);

            return projects.Select(p => CreateFindProjectsViewModel(p));
        }

        public async Task<IEnumerable<FindProjectsViewModel>> MyProjects(string userId)
        {
            var projects = await _context
                .Projects
                .Where(p => p.OwnerId == userId && p.Status != ProjectStatus.Deleted)
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.BannerImage)
                .Include(p => p.ParticipantCounts)                
                .OrderBy(p => p.DisplayPriority)
                .ToListAsync();

            var viewModels = projects.Select(p => 
                {
                    var viewModel = CreateFindProjectsViewModel(p);
                    viewModel.CanSendProjectEmail = this.CanSendProjectEmail(p);
                    return viewModel;
                })
                .ToList();

            return viewModels;
        }

        public async Task<IEnumerable<FindProjectsViewModel>> ParticipatingInProjects(string userId)
        {
            var projects = await _context
                .ProjectParticipants
                .Where(pa => pa.UserId == userId && pa.Project.Status != ProjectStatus.Deleted)
                .Select(pa => pa.Project)
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.BannerImage)
                .Include(p => p.ParticipantCounts)
                .Include(p => p.Participants)
                .OrderBy(p => p.DisplayPriority)                
                .ToListAsync();

            var viewModels = projects
                .Select(p => 
                {
                    var viewModel = CreateFindProjectsViewModel(p);
                    viewModel.SubscribedToEmails = p.Participants
                        .First(pa => pa.UserId == userId)
                        .SubscribedToProjectEmails;
                    return viewModel;
                })
                .ToList();

            return viewModels;
        }

        private FindProjectsViewModel CreateFindProjectsViewModel(Models.Project project) =>
            new FindProjectsViewModel()
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                ProjectNameUriPart = GetProjectNameNormalized(project.Name),
                ProjectProposal = project.Proposal,
                CategoryName = project.Category.Name,
                CategoryColorHex = project.Category.ColorHex,
                LocationName = project.Location != null ? project.Location.Name : string.Empty,
                BannerImagePath = project.BannerImage != null ? _imageService.GetUrl(project.BannerImage) : $"/images/default_banners/{project.Category.Name}.jpg",
                BannerImageDescription = project.BannerImage != null ? project.BannerImage.Description : string.Empty,
                Target = project.Target,
                Participants = project.ParticipantCounts.Count,
                Remaining = project.RemainingTime,
                Status = project.Status,
                Start = project.Start,
                End = project.End
            };

        public async Task<bool> ToggleNewsletterSubscription(int projectId, string userId)
        {
            ProjectParticipant participant = await _context
                .ProjectParticipants
                .Include(p => p.Project)
                .FirstAsync(p => p.ProjectId == projectId && p.UserId == userId);

            if (participant == null)
            {
                throw new ArgumentException($"User {userId} does not participate in project {projectId}");
            }

            var newValue = !participant.SubscribedToProjectEmails;

            participant.SubscribedToProjectEmails = newValue;
            await _context.SaveChangesAsync();

            return newValue;
        }
    }
}