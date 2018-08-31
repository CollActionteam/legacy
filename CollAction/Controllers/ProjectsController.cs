using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CollAction.Data;
using CollAction.Models;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using CollAction.Helpers;
using CollAction.Services;
using CollAction.Models.ProjectViewModels;
using System.Net;
using System.Linq.Expressions;

namespace CollAction.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<ProjectsController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IProjectService _projectService;
        private readonly IEmailSender _emailSender;

        public ProjectsController(ApplicationDbContext context, IStringLocalizer<ProjectsController> localizer, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment, IProjectService projectService, IEmailSender emailSender)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _projectService = projectService;
            _emailSender = emailSender;
        }

        public ViewResult StartInfo()
            => View();

        public IActionResult Find()
            => View();

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IEnumerable<DisplayProjectViewModel> items = await _projectService.GetProjectDisplayViewModels(p => p.Id == id && p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted);
            if (items.Count() == 0)
            {
                return NotFound();
            }
            DisplayProjectViewModel displayProject = items.First();
            string userId = (await _userManager.GetUserAsync(User))?.Id;
            displayProject.IsUserCommitted = userId != null && (await _projectService.GetParticipant(userId, displayProject.Project.Id) != null);

            return View(displayProject);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await  _userManager.GetUserAsync(User);

            if (user == null) throw new InvalidOperationException("User doesn't have a UserIdClaimType. Can't determine user information.");

            return View(new CreateProjectViewModel
            {
                ProjectStarterFirstName = user.FirstName,
                ProjectStarterLastName = user.LastName,
                Start = DateTime.UtcNow.Date.AddDays(7), // A week from today
                End = DateTime.UtcNow.Date.AddDays(7).AddMonths(1), // A month after start
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description"),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            // Make sure the project name is unique.
            if (await _context.Projects.AnyAsync(p => p.Name == model.Name))
            {
                ModelState.AddModelError("Name", _localizer["A project with the same name already exists."]);
            }

            // If there are image descriptions without corresponding image uploads, warn the user.
            if (model.BannerImageUpload == null && !string.IsNullOrWhiteSpace(model.BannerImageDescription))
            {
                ModelState.AddModelError("BannerImageDescription", _localizer["You can only provide a 'Banner Image Description' if you upload a 'Banner Image'."]);
            }
            if (model.DescriptiveImageUpload == null && !string.IsNullOrWhiteSpace(model.DescriptiveImageDescription))
            {
                ModelState.AddModelError("DescriptiveImageDescription", _localizer["You can only provide a 'DescriptiveImage Description' if you upload a 'DescriptiveImage'."]);
            }

            if (!ModelState.IsValid) {
                model.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
                return View(model);
            }

            var project = new Project
            {
                OwnerId = (await _userManager.GetUserAsync(User)).Id,
                Name = model.Name,
                Description = model.Description,
                Proposal = model.Proposal,
                Goal = model.Goal,
                CreatorComments = model.CreatorComments,
                CategoryId = model.CategoryId,
                LocationId = model.LocationId,
                Target = model.Target,
                Start = model.Start,
                End = model.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                BannerImage = null
            };

            var bannerImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "bannerimages"));
            project.BannerImage = await bannerImageManager.CreateOrReplaceImageFileIfNeeded(project.BannerImage, model.BannerImageUpload, model.BannerImageDescription);

            var descriptiveImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "descriptiveimages"));
            project.DescriptiveImage = await descriptiveImageManager.CreateOrReplaceImageFileIfNeeded(project.DescriptiveImage, model.DescriptiveImageUpload, model.DescriptiveImageDescription);

            _context.Add(project);

            // Save the main project
            await _context.SaveChangesAsync();

            // Save project related items (now that we've got a project id)
            project.SetDescriptionVideoLink(_context, model.DescriptionVideoLink);
            await project.SetTags(_context, model.Hashtag?.Split(';') ?? new string[0]);

            await _context.SaveChangesAsync();

            await _projectService.RefreshParticipantCountMaterializedView();

            // Notify admins and creator through e-mail
            string confirmationEmail =
                "Hi!<br>" +
                "<br>" +
                "Thanks for submitting a project on www.collaction.org!<br>" +
                "The CollAction Team will review your project as soon as possible - if it meets all the criteria we'll publish the project on the website and will let you know, so you can start promoting it! If we have any additional questions or comments, we'll reach out to you by email.<br>" +
                "<br>" +
                "Thanks so much for driving the CollAction / crowdacting movement!<br>" +
                "<br>" +
                "Warm regards,<br>" +
                "The CollAction team";
            string subject = $"Thank you for participating in the \"{project.Name}\" project on CollAction";

            ApplicationUser user = await _userManager.GetUserAsync(User);
            _emailSender.SendEmail(user.Email, subject, confirmationEmail);

            string confirmationEmailAdmin =
                "Hi!<br>" +
                "<br>" +
                $"There's a new project waiting for approval: {project.Name}<br>" +
                "Warm regards,<br>" +
                "The CollAction team";

            var administrators = await _userManager.GetUsersInRoleAsync(Constants.AdminRole);
            foreach (var admin in administrators)
                _emailSender.SendEmail(admin.Email, subject, confirmationEmailAdmin);

            return View("ThankYouCreate", new ThankYouCreateProjectViewModel()
            {
                Name = project.Name
            });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(User) != project.OwnerId)
            {
                return Forbid();
            }

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(m => m.Id == id);

            if (_userManager.GetUserId(User) != project.OwnerId)
            {
                return Forbid();
            }

            project.Status = ProjectStatus.Deleted;
            await _context.SaveChangesAsync();
            return RedirectToAction("Find");
        }

        [Authorize]
        public async Task<IActionResult> Commit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project project =  await _projectService.GetProjectById(id.Value); 
            if (project == null)
            {
                return NotFound();
            }

            var commitProjectViewModel = new CommitProjectViewModel
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                ProjectProposal = project.Proposal,
                IsUserCommitted = (await _projectService.GetParticipant((await _userManager.GetUserAsync(User)).Id, project.Id) != null),
                IsActive = project.IsActive
            };

            return View(commitProjectViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Commit(int id, CommitProjectViewModel commitProjectViewModel)
        {
            if (id != commitProjectViewModel.ProjectId)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            bool success = await _projectService.AddParticipant(user.Id, commitProjectViewModel.ProjectId);

            if (success)
            {
                string projectUrl = Url.Action("Details", "Projects", new { id = commitProjectViewModel.ProjectId }, HttpContext.Request.Scheme); 
                var systemUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                var userDescription = user?.FirstName ?? "";
                string confirmationEmail =
                    $"Hi {userDescription}!<br><br>" +
                    "Thank you for participating in a CollAction project!<br><br>" +
                    "In crowdacting, we only act collectively when we meet the target before the deadline, so please feel very welcome to share this project on social media through the social media buttons below and on the <a href="+projectUrl+">project page</a>!<br><br>" +
                    "We'll keep you updated on the project. Also feel free to Like us on <a href=\"https://www.facebook.com/collaction.org/\">Facebook</a> to stay up to date on everything CollAction!<br><br>" +
                    "Warm regards,<br>The CollAction team<br><br>" +
                    "PS: Did you know you can start your own project on <a href=\"https://collaction.org/start\">www.collaction.org/start</a> ?<br><br>"+
                    "<span style='#share-buttons img {}'>"+
                    "<div id='share-buttons'>"+
                    "<p>Multiply your impact and share the project with the buttons below 🙂</p>"+
                    "<a href=https://www.facebook.com/sharer/sharer.php?u="+projectUrl+">"+
                    "<img style='width: 25px; padding: 5px;border: 0;box-shadow: 0;display: inline;' src="+systemUrl+"/images/social/facebook.png alt='Facebook' />"+
                    "</a>"+
                    "<a href=\"http://www.linkedin.com/shareArticle?mini=true&url="+projectUrl+"&title="+WebUtility.UrlEncode(commitProjectViewModel.ProjectName)+"\" target=\"_blank\">"+
                    "<img style='width: 25px; padding: 5px;border: 0;box-shadow: 0;display: inline;' src="+systemUrl+"/images/social/linkedin.png alt='LinkedIn' />"+
                    "</a>"+
                    "<a href=\"https://twitter.com/intent/tweet?text="+WebUtility.UrlEncode(commitProjectViewModel.ProjectName)+"&url="+projectUrl+"\" target=\"_blank\">"+
                    "<img style='width: 25px; padding: 5px;border: 0;box-shadow: 0;display: inline;' src="+systemUrl+"/images/social/twitter.png alt='Twitter' />"+
                    "</a>"+
                    "</div>"+
                    "</span>";
                string subject = $"Thank you for participating in the \"{commitProjectViewModel.ProjectName}\" project on CollAction";
                _emailSender.SendEmail(user.Email, subject, confirmationEmail);
                return View("ThankYouCommit", commitProjectViewModel);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<JsonResult> FindProjects(int? categoryId, int? statusId)
        {
            Expression<Func<Project, bool>> projectExpression = (p =>
                p.Status != ProjectStatus.Hidden &&
                p.Status != ProjectStatus.Deleted &&
                ((categoryId != null && categoryId >= 0) ? p.CategoryId == categoryId : true));

            Expression<Func<Project, bool>> statusExpression;
            switch (statusId)
            {
                case (int)ProjectExternalStatus.Open: statusExpression = (p => p.Status == ProjectStatus.Running && p.Start <= DateTime.UtcNow && p.End >= DateTime.UtcNow); break;
                case (int)ProjectExternalStatus.Closed: statusExpression = (p => (p.Status == ProjectStatus.Running && p.End < DateTime.UtcNow) || p.Status == ProjectStatus.Successful || p.Status == ProjectStatus.Failed); break;
                case (int)ProjectExternalStatus.ComingSoon: statusExpression = (p => p.Status == ProjectStatus.Running && p.Start > DateTime.UtcNow); break;
                default: statusExpression = (p => true); break;
            }

            var projects = await _projectService.FindProjects(
                Expression.Lambda<Func<Project, bool>>(Expression.AndAlso(projectExpression.Body, Expression.Invoke(statusExpression, projectExpression.Parameters[0])), projectExpression.Parameters[0]));

            return Json(projects);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SendProjectEmail(int id)
        {
            Project project = await _projectService.GetProjectById(id);
            ApplicationUser user = await _userManager.GetUserAsync(User);

            SendProjectEmail model = new SendProjectEmail()
            {
                ProjectId = project.Id,
                Project = project,
                EmailsAllowedToSend = _projectService.NumberEmailsAllowedToSend(project),
                SendEmailsUntil = _projectService.CanSendEmailsUntil(project)
            };

            if (model.Project.OwnerId != user.Id)
                return Unauthorized();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendProjectEmailPerform([Bind("ProjectId", "Subject", "Message")]SendProjectEmail model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(SendProjectEmail), new { id = model.ProjectId });

            model.Project = await _projectService.GetProjectById(model.ProjectId);
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (model.Project.OwnerId != user.Id)
                return Unauthorized();

            await _projectService.SendProjectEmail(model.Project, model.Subject, model.Message, Request, Url);
            return RedirectToAction(nameof(ManageController.Index), "Manage");
        }

        [HttpGet]
        public async Task<IActionResult> ChangeSubscriptionFromToken(ChangeSubscriptionFromTokenViewModel unsubscribeViewmodel)
        {
            ProjectParticipant participant = await _context
                .ProjectParticipants
                .Include(p => p.Project)
                .FirstAsync(p => p.ProjectId == unsubscribeViewmodel.ProjectId && p.UserId == unsubscribeViewmodel.UserId);

            if (participant != null && participant.UnsubscribeToken == new Guid(unsubscribeViewmodel.UnsubscribeToken))
                return View(participant);
            else
                return Unauthorized();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSubscriptionFromTokenPerform(ChangeSubscriptionFromTokenViewModel unsubscribeViewmodel)
        {
            ProjectParticipant participant = await _context
                .ProjectParticipants
                .Include(p => p.Project)
                .FirstAsync(p => p.ProjectId == unsubscribeViewmodel.ProjectId && p.UserId == unsubscribeViewmodel.UserId);

            if (participant != null && participant.UnsubscribeToken == new Guid(unsubscribeViewmodel.UnsubscribeToken))
            {
                participant.SubscribedToProjectEmails = !participant.SubscribedToProjectEmails;
                await _context.SaveChangesAsync();

                return View(nameof(ChangeSubscriptionFromToken), participant);
            }
            else
                return Unauthorized();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangeSubscriptionFromAccount(int id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            ProjectParticipant participant = await _context
                .ProjectParticipants
                .Include(p => p.Project)
                .FirstAsync(p => p.ProjectId == id && p.UserId == user.Id);

            if (participant != null)
                return View(participant);
            else
                return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSubscriptionFromAccountPerform(int id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            ProjectParticipant participant = await _context
                .ProjectParticipants
                .Include(p => p.Project)
                .FirstAsync(p => p.ProjectId == id && p.UserId == user.Id);

            if (participant != null)
            {
                participant.SubscribedToProjectEmails = !participant.SubscribedToProjectEmails;
                await _context.SaveChangesAsync();

                return View(nameof(ChangeSubscriptionFromAccount), participant);
            }
            else
                return Unauthorized();
        }

        [HttpGet]
        public async Task<JsonResult> GetCategories()
            => Json(new[] { new CategoryViewModel() { Id = -1, Name = "All" } }.Concat(
                await _context
                    .Categories
                    .Where(c => c.Name != "Other")
                    .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name })
                    .OrderBy(c => c.Name)
                    .ToListAsync()));

        [HttpGet]
        public JsonResult GetStatuses()
            => Json(
                Enum.GetValues(typeof(ProjectExternalStatus))
                    .Cast<ProjectExternalStatus>()
                    .Select(status => new ExternalStatusViewModel() { Id = (int)status, Status = status.ToString() }));
    }
}
