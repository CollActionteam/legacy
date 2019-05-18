using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CollAction.Data;
using CollAction.Models;
using CollAction.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using CollAction.Models.ProjectViewModels;
using System.Net;
using System.Linq.Expressions;
using CollAction.Services.Project;
using CollAction.Services.Email;
using CollAction.Services.Image;

namespace CollAction.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<ProjectsController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IProjectService _projectService;
        private readonly IParticipantsService _participantsService;
        private readonly IEmailSender _emailSender;
        private readonly IImageService _imageService;

        public ProjectsController(ApplicationDbContext context, IStringLocalizer<ProjectsController> localizer, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment, IProjectService projectService, IParticipantsService participantsService, IEmailSender emailSender, IImageService imageService)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _projectService = projectService;
            _participantsService = participantsService;
            _emailSender = emailSender;
            _imageService = imageService;
        }

        public ViewResult StartInfo()
            => View();

        public IActionResult Find()
            => View();

        public async Task<IActionResult> Details(string name, int id)
        {
            IEnumerable<DisplayProjectViewModel> items = await _projectService.GetProjectDisplayViewModels(p => p.Id == id && p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted).ToListAsync();
            if (items.Count() == 0)
            {
                return NotFound();
            }
            DisplayProjectViewModel displayProject = items.First();
            string userId = (await _userManager.GetUserAsync(User))?.Id;
            displayProject.IsUserCommitted = userId != null && (await _participantsService.GetParticipant(userId, displayProject.Project.Id) != null);
            
            ViewData["CurrentUser"] = await _userManager.GetUserAsync(User);
            return View(displayProject);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

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

            if (model.BannerImageUpload != null)
            {
                project.BannerImage = await _imageService.UploadImage(project.BannerImage, model.BannerImageUpload, model.BannerImageDescription ?? string.Empty);
                _context.ImageFiles.Add(project.BannerImage);
            }
            if (model.DescriptiveImageUpload != null)
            {
                project.DescriptiveImage = await _imageService.UploadImage(project.DescriptiveImage, model.DescriptiveImageUpload, model.DescriptiveImageDescription ?? string.Empty);
                _context.ImageFiles.Add(project.DescriptiveImage);
            }

            _context.Add(project);

            // Save the main project
            await _context.SaveChangesAsync();

            // Save project related items (now that we've got a project id)
            project.SetDescriptionVideoLink(_context, model.DescriptionVideoLink);
            await project.SetTags(_context, model.Hashtag?.Split(';') ?? new string[0]);

            await _context.SaveChangesAsync();

            await _participantsService.RefreshParticipantCountMaterializedView();

            // Notify admins and creator through e-mail
            string confirmationEmail =
                "Hi!<br>" +
                "<br>" +
                "Thanks for submitting a project on www.collaction.org!<br>" +
                "The CollAction Team will review your project as soon as possible - if it meets all the criteria we'll publish the project on the website and will let you know, so you can start promoting it! If we have any additional questions or comments, we'll reach out to you by email.<br>" +
                "Also, did you know we have a <a href=\"https://docs.google.com/document/d/1JK058S_tZXntn3GzFYgiH3LWV5e9qQ0vXmEyV-89Tmw\">Project Starter Handbook</a> with tips and tricks on how to start, run, and finish a project on CollAction?" +
                "<br>" +
                "<br>" +
                "Thanks so much for driving the CollAction / crowdacting movement!<br>" +
                "<br>" +
                "Warm regards,<br>" +
                "The CollAction team";
            string subject = $"Thank you for submitting \"{project.Name}\" on CollAction";

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

            return LocalRedirect($"~/Projects/Create/{_projectService.GetProjectNameNormalized(project.Name)}/{project.Id}/thankyou");
        }

        [Authorize]
        public IActionResult ThankYouCreate(string name, int? id)
        {
            var project = _context.Projects.SingleOrDefault(m => m.Id == id.Value);
            if (project == null)
            {
                return NotFound();
            }
            return View(new ThankYouCreateProjectViewModel
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Commit(int projectId, string email = null)
        {
            var project =  await _projectService.GetProjectById(projectId); 
            if (project == null)
            {
                return NotFound();
            }

            var projectNameUriPart = _projectService.GetProjectNameNormalized(project.Name);
            var systemUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
            var projectUrl = Url.Action("Details", "Projects", new { id = project.Id }, HttpContext.Request.Scheme); 

            var loggedInUser = await _userManager.GetUserAsync(User);
            
            var result = loggedInUser != null
                ? await _participantsService.AddLoggedInParticipant(projectId, loggedInUser.Id)
                : await _participantsService.AddAnonymousParticipant(projectId, email);

            var emailHelper = new CommitEmailHelper(project, result, loggedInUser, systemUrl, projectUrl);

            var emailAddress = loggedInUser?.Email 
                ?? email 
                ?? throw new ArgumentException("No e-mail adres specified");

            _emailSender.SendEmail(emailAddress, emailHelper.GenerateSubject(), emailHelper.GenerateEmail());

            return LocalRedirect($"~/Projects/{projectNameUriPart}/{projectId}/thankyou");
        }

        [HttpGet]
        public IActionResult ThankYouCommit(int id, string name)
        {
            var project = _context.Projects.SingleOrDefault(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var model = new CommitProjectViewModel()
            {
                ProjectId = id,
                ProjectName = project.Name,
                ProjectNameUriPart = _projectService.GetProjectNameNormalized(project.Name)
            };
            return View(nameof(ThankYouCommit), model);
        }

        [HttpGet]
        public async Task<IActionResult> FindProject(int projectId)
        {
            var project = await _projectService.FindProject(projectId);

            if (project == null)
            {
                return NotFound();
            }

            return Json(project);
        }        

        [HttpGet]
        public async Task<JsonResult> FindProjects(int? categoryId, int? statusId, int? limit, int? start)
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

            var projects = await _projectService.FindProjects(Expression.Lambda<Func<Project, bool>>(Expression.AndAlso(projectExpression.Body, Expression.Invoke(statusExpression, projectExpression.Parameters[0])), projectExpression.Parameters[0]), limit, start).ToListAsync();

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
