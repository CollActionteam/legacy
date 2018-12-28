using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Email;
using CollAction.Services.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<ProjectService> _stringLocalizer;
        private static Regex _spaceRemoveRegex = new Regex(@"\s", RegexOptions.Compiled);
        private static Regex _invalidCharRemoveRegex = new Regex(@"[^a-z0-9\s-_]",RegexOptions.Compiled);
        private static Regex _doubleDashRemoveRegex = new Regex(@"([-_]){2,}", RegexOptions.Compiled);

        public ProjectService(ApplicationDbContext context, IEmailSender emailSender, IImageService imageService, IOptions<ProjectEmailOptions> projectEmailOptions, ILogger<ProjectService> logger, IStringLocalizer<ProjectService> stringLocalizer, UserManager<ApplicationUser> userManager)
        {
            _imageService = imageService;
            _context = context;
            _projectEmailOptions = projectEmailOptions.Value;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
        }

        public Task<Models.Project> GetProjectById(int id)
            => _context.Projects.SingleOrDefaultAsync(p => p.Id == id);

        public async Task<bool> AddParticipant(string userId, int projectId)
        {
            Models.Project project = await GetProjectById(projectId);
            if (project == null || !project.IsActive)
            {
                return false;
            }

            var participant = new ProjectParticipant
            {
                UserId = userId,
                ProjectId = projectId,
                SubscribedToProjectEmails = true,
                UnsubscribeToken = Guid.NewGuid()
            };

            try
            {
                _context.Add(participant);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            await RefreshParticipantCountMaterializedView();

            return true;
        }

        public async Task<ProjectParticipant> GetParticipant(string userId, int projectId)
        {
            return await _context.ProjectParticipants.SingleOrDefaultAsync(p => p.ProjectId == projectId && p.UserId == userId);
        }

        public IQueryable<FindProjectsViewModel> FindProjects(Expression<Func<Models.Project, bool>> filter, int? limit)
        {
            IQueryable<Models.Project> projects = _context.Projects
                .Where(filter)
                .OrderBy(p => p.DisplayPriority);

            if (limit.HasValue)
                projects = projects.Take(limit.Value);

            return projects
                .Select(project =>
                            new FindProjectsViewModel(_stringLocalizer)
                            {
                                ProjectId = project.Id,
                                ProjectName = project.Name,
                                ProjectNameUriPart = GetProjectNameNormalized(project.Name),
                                ProjectProposal = project.Proposal,
                                CategoryName = project.Category.Name,
                                CategoryColorHex = project.Category.ColorHex,
                                LocationName = project.Location.Name,
                                BannerImagePath = project.BannerImage != null ? _imageService.GetUrl(project.BannerImage) : $"/images/default_banners/{project.Category.Name}.jpg",
                                BannerImageDescription = project.BannerImage != null ? project.BannerImage.Description : string.Empty,
                                Target = project.Target,
                                Participants = project.ParticipantCounts.Count,
                                Remaining = project.RemainingTime,
                                DescriptiveImagePath = project.DescriptiveImage == null ? null : _imageService.GetUrl(project.DescriptiveImage),
                                DescriptiveImageDescription = project.Description == null ? string.Empty : project.DescriptiveImage.Description,
                                Status = project.Status,
                                Start = project.Start,
                                End = project.End
                            });
        }

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

        public async Task<string> GenerateParticipantsDataExport(int projectId)
        {
            Models.Project project = await _context
                .Projects
                .Where(p => p.Id == projectId)
                .Include(p => p.Participants).ThenInclude(p => p.User)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync();
            if (project == null)
                return null;
            else
                return string.Join("\r\n", GetParticipantsCsv(project));
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

        public Task RefreshParticipantCountMaterializedView()
            => _context.Database.ExecuteSqlCommandAsync(@"REFRESH MATERIALIZED VIEW CONCURRENTLY ""ProjectParticipantCounts"";");

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

        private IEnumerable<string> GetParticipantsCsv(Models.Project project)
        {
            yield return "first-name;last-name;email";
            yield return GetParticipantCsvLine(project.Owner);
            foreach (ProjectParticipant participant in project.Participants)
                yield return GetParticipantCsvLine(participant.User);
        }
        
        private static string ToUrlSlug(string value)
        {                      
            value = value.ToLowerInvariant();
            value = _spaceRemoveRegex.Replace(value, "-");
            value = _invalidCharRemoveRegex.Replace(value, "");
            value = value.Trim('-', '_');
            value = _doubleDashRemoveRegex.Replace(value, "$1");
            if(value.Length == 0)
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

        private string GetParticipantCsvLine(ApplicationUser user)
            => $"{EscapeCsv(user?.FirstName)};{EscapeCsv(user?.LastName)};{EscapeCsv(user?.Email)}";

        private string EscapeCsv(string str)
            => $"\"{str?.Replace("\"", "\"\"")}\"";
    }
}