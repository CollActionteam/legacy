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
using System.Linq.Expressions;
using System.Net;

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
        {
            return View();
        }

        public async Task<IActionResult> Find(FindProjectViewModel model)
        {
            if (model.SearchText == null)
            {
                return View(new FindProjectViewModel
                {
                    OwnerId = (await _userManager.GetUserAsync(User))?.Id,
                    Projects = await _projectService.GetProjectDisplayViewModels(p => p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted)
                });
            }

            model.OwnerId = (await _userManager.GetUserAsync(User))?.Id;
            model.Projects = await _projectService.GetProjectDisplayViewModels(p => p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted &&
                (p.Name.Contains(model.SearchText) || p.Description.Contains(model.SearchText) || p.Goal.Contains(model.SearchText)));
            return View(model);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> DetailsById(int? id)
        {
            var project =  await _projectService.GetProjectById(id); 
            if (project == null)
            {
                return NotFound();
            }
            string validUrlForProjectName = WebUtility.UrlEncode(project.Name);
            if(String.IsNullOrEmpty(validUrlForProjectName))
            {
                return NotFound();
            }
            return LocalRedirect("~/projects/"+validUrlForProjectName+"/details");
        }
        
        public async Task<IActionResult> Details(string name)
        {
            List<DisplayProjectViewModel> items = await DisplayProjectViewModel.GetViewModelsWhere(_context, p => p.Name == WebUtility.UrlDecode(name) && p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted);
            if (items.Count == 0)
            {
                return NotFound();
            }
            DisplayProjectViewModel displayProject = items.First();
            string userId = (await _userManager.GetUserAsync(User))?.Id;
            if (userId != null && (await _projectService.GetParticipant(userId, displayProject.Project.Id) != null))
            {
                displayProject.IsUserCommitted = true;
            }

            return View(displayProject);
        }

        public IActionResult Embed()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View(new CreateProjectViewModel
            {
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
                ModelState.AddModelError("Name", _localizer["Een project met deze naam bestaat al. Kies ajb een nieuwe naam."]);
            }

            // If there are image descriptions without corresponding image uploads, warn the user.
            if (model.BannerImageUpload == null && !string.IsNullOrWhiteSpace(model.BannerImageDescription))
            {
                ModelState.AddModelError("BannerImageDescription", _localizer["Je kunt deze beschrijving alleen toevoegen als je een banner afbeelding toevoegt."]);
            }
            if (model.DescriptiveImageUpload == null && !string.IsNullOrWhiteSpace(model.DescriptiveImageDescription))
            {
                ModelState.AddModelError("DescriptiveImageDescription", _localizer["Je kunt deze beschrijving alleen toevoegen als je een afbeelding toevoegt."]);
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
                CategoryId = (await _context
                    .Categories
                    .SingleAsync(c => c.Name == "Friesland")).Id,
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
            await _context.SaveChangesAsync();

            // Save project related items (now that we've got a project id
            project.SetDescriptionVideoLink(_context, model.DescriptionVideoLink);
            project.SetTags(_context, model.Hashtag?.Split(';') ?? new string[0]);

            await _context.SaveChangesAsync();

            // Notify admins and creator through e-mail
            //"Hi!<br>" +
            //"<br>" +
            //"Thanks for submitting a project on www.collaction.org!<br>" +
            //"The CollAction Team will review your project as soon as possible – if it meets all the criteria we’ll publish the project on the website and will let you know, so you can start promoting it! If we have any additional questions or comments, we’ll reach out to you by email.<br>" +
            //"<br>" +
            //"Thanks so much for driving the CollAction / crowdacting movement!<br>" +
            //"<br>" +
            //"Warm regards,<br>" +
            //"The CollAction team";
            string confirmationEmail =
                "Hi!<br>" +
                "<br>" +
                "Dank voor het insturen van een project op www.freonen.collaction.org!<br>" +
                "Het Freonen/CollAction team gaat er zorgvuldig naar kijken. Als het in lijn is met de criteria zullen we het online zetten – we laten het weten wanneer dit gebeurt.  Als we nog aanvullende vragen of opmerkingen hebben, komen we bij je terug via email.<br>" +
                "<br>" +
                "Nogmaals dank voor je aanmelding!<br>" +
                "<br>" +
                "Warme groet,<br>" +
                "Het Freonen team";

            //Confirmation email - start project {project.Name}
            string subject = $"Dank voor het insturen van een project: {project.Name}";

            ApplicationUser user = await _userManager.GetUserAsync(User);
            await _emailSender.SendEmailAsync(user.Email, subject, confirmationEmail);

            //"Hi!<br>" +
            //"<br>" +
            //$"There's a new project waiting for approval: {project.Name}<br>" +
            //"Warm regards,<br>" +
            //"The CollAction team";
            string confirmationEmailAdmin =
                "Hi!<br>" +
                "<br>" +
                $"Er is een nieuw project aangemaakt: {project.Name}. Ga naar het CollAction management dashboard om het te checken en goed (of af) te keuren. Daar vind je ook de contactgegevens van de projectstarter.<br>" +
                "Vriendelijke groet,<br>" +
                "Het CollAction team";

            var administrators = await _userManager.GetUsersInRoleAsync("admin");
            foreach (var admin in administrators)
                await _emailSender.SendEmailAsync(admin.Email, subject, confirmationEmailAdmin);

            string validUrlForProjectName = WebUtility.UrlEncode(project.Name);
            if(String.IsNullOrEmpty(validUrlForProjectName))
             {
                return NotFound();
            }
            return LocalRedirect("~/Projects/Create/"+validUrlForProjectName+"/thankyou");
        }

        [Authorize]
        public IActionResult ThankYouCreate(string name)
        {
            return View(new ThankYouCreateProjectViewModel
            {
                Name = WebUtility.UrlDecode(name)
             });
         }

        // GET: Projects/Delete/5
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
        public async Task<IActionResult> Commit(string name)
        {
            var project =  await _projectService.GetProjectByName(WebUtility.UrlDecode(name)); 
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
        public async Task<IActionResult> Commit(CommitProjectViewModel commitProjectViewModel)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            bool success = await _projectService.AddParticipant(user.Id, commitProjectViewModel.ProjectId);

            if (success)
            {
                    //"Hi!<br><br>" +
                    //"Thank you for participating in a CollAction project!<br><br>" +
                    //"In crowdacting, we only act collectively when we meet the target before the deadline, so please feel very welcome to share this project on social media through the social media buttons on the project page!<br><br>" +
                    //"We’ll keep you updated on the project. Also feel free to Like us on <a href=\"https://www.facebook.com/collaction.org/\">Facebook</a> to stay up to date on everything CollAction!<br><br>" +
                    //"Warm regards,<br>The CollAction team";
                string confirmationEmail = 

                    "Hi!<br><br>" +
                    "Dank voor je deelname aan een Freonen crowdacting project!<br><br>" +
                    "Bij crowdacting komen we alleen collectief in actie als het target wordt gehaald voor de deadline, dus deel dit project vooral met je netwerk met de buttons op de project pagina!<br><br>" +
                    "We houden je op de hoogte van het project via de mail. Als je op de hoogte wil blijven van alles Freonen, like @freonen dan op Facebook. Wil je meer leren over CollAction en crowdacting, like dan ook @collaction.org!<br><br>" +
                    "Warme groet,<br>Het FreonenTeam";

                // Thank you for participating in a CollAction project!
                string subject = "Dank voor je deelname aan een Freonen crowdacting project!";
                await _emailSender.SendEmailAsync(user.Email, subject, confirmationEmail);
                return View("ThankYouCommit", commitProjectViewModel);
            }
            else
            {
                return View("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult ThankYouCommit(string name)
        {
            if(name!=null)
            {
                CommitProjectViewModel model = new CommitProjectViewModel()
                {
                    ProjectName = WebUtility.UrlDecode(name)
                };
                return View("ThankYouCommit", model);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTileProjects(int? categoryId, int? statusId)
        {
            Expression<Func<Project, bool>> projectExpression = (p =>
                p.Status != ProjectStatus.Hidden &&
                p.Status != ProjectStatus.Deleted &&
                p.Category.Name.Equals("Friesland"));

            Expression<Func<Project, bool>> statusExpression;
            switch (statusId)
            {
                case (int)ProjectExternalStatus.Open: statusExpression = (p => p.Status == ProjectStatus.Running && p.Start <= DateTime.UtcNow && p.End >= DateTime.UtcNow); break;
                case (int)ProjectExternalStatus.Closed: statusExpression = (p => (p.Status == ProjectStatus.Running && p.End < DateTime.UtcNow) || p.Status == ProjectStatus.Successful || p.Status == ProjectStatus.Failed); break;
                case (int)ProjectExternalStatus.ComingSoon: statusExpression = (p => p.Status == ProjectStatus.Running && p.Start > DateTime.UtcNow); break;
                default: statusExpression = (p => true); break;
            }
            
            var projects = await _projectService.GetTileProjectViewModels(
                Expression.Lambda<Func<Project, bool>>(Expression.AndAlso(projectExpression.Body, Expression.Invoke(statusExpression, projectExpression.Parameters[0])), projectExpression.Parameters[0]));
            
            return Json(projects);
        }        

        [HttpGet]
        public async Task<JsonResult> GetTileProject(int projectId)
        {
            Expression<Func<Project, bool>> projectExpression= (p =>
                p.Status != ProjectStatus.Hidden &&
                p.Status != ProjectStatus.Deleted &&
                p.Id == projectId);

            Expression<Func<Project, bool>> statusExpression = (p => true);

            var projects = await _projectService.GetTileProjectViewModels(projectExpression);
            return Json(projects);
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
