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
using System.Data.SqlClient;
using Npgsql;
using Microsoft.AspNetCore.Hosting;
using CollAction.Helpers;
using CollAction.Services;
using System.Text.RegularExpressions;
using CollAction.Models.ProjectViewModels;
using System.Linq.Expressions;

namespace CollAction.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<ProjectsController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IProjectService _service;
        private readonly IEmailSender _emailSender;

        public ProjectsController(ApplicationDbContext context, IStringLocalizer<ProjectsController> localizer, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment, IProjectService service, IEmailSender emailSender)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _service = service;
            _emailSender = emailSender;
        }

        public ViewResult StartInfo()
        {
            return View();
        }

        // GET: Project/Find
        public async Task<IActionResult> Find(FindProjectViewModel model)
        {
            if (model.SearchText == null)
            {
                return View(new FindProjectViewModel
                {
                    OwnerId = (await _userManager.GetUserAsync(User))?.Id,
                    Projects = await DisplayProjectViewModel.GetViewModelsWhere(_context, p => p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted)
                });
            }

            model.OwnerId = (await _userManager.GetUserAsync(User))?.Id;
            model.Projects = await DisplayProjectViewModel.GetViewModelsWhere(_context, p => p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted &&
                (p.Name.Contains(model.SearchText) || p.Description.Contains(model.SearchText) || p.Goal.Contains(model.SearchText)));
            return View(model);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<DisplayProjectViewModel> items = await DisplayProjectViewModel.GetViewModelsWhere(_context, p => p.Id == id && p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted);
            if (items.Count == 0)
            {
                return NotFound();
            }
            DisplayProjectViewModel displayProject = items.First();
            string userId = (await _userManager.GetUserAsync(User))?.Id;
            if (userId != null && (await _service.GetParticipant(userId, displayProject.Project.Id) != null))
            {
                displayProject.IsUserCommitted = true;
            }

            return View(displayProject);
        }

        // GET: Projects/Create
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

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                Goal = model.Goal,
                Proposal = model.Proposal,
                CreatorComments = model.CreatorComments,
                CategoryId = _context
                    .Categories
                    .FirstOrDefault(c => c.Name == "Friesland")
                    .Id,
                LocationId = model.LocationId,
                Target = model.Target,
                Start = model.Start,
                End = model.End,
                BannerImage = null
            };

            var bannerImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "bannerimages"));
            project.BannerImage = await bannerImageManager.CreateOrReplaceImageFileIfNeeded(project.BannerImage, model.BannerImageUpload, model.BannerImageDescription);

            var descriptiveImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "descriptiveimages"));
            project.DescriptiveImage = await descriptiveImageManager.CreateOrReplaceImageFileIfNeeded(project.DescriptiveImage, model.DescriptiveImageUpload, model.DescriptiveImageDescription);

            _context.Add(project);
            await _context.SaveChangesAsync();

            project.SetDescriptionVideoLink(_context, model.DescriptionVideoLink);

            // Only call this once we have a valid Project.Id
            await project.SetTags(_context, model.Hashtag?.Split(';') ?? new string[0]);

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
            string subject = $"Confirmation email - start project {project.Name}";

            ApplicationUser user = await _userManager.GetUserAsync(User);
            await _emailSender.SendEmailAsync(user.Email, subject, confirmationEmail);

            string confirmationEmailAdmin =
                "Hi!<br>" +
                "<br>" +
                $"There's a new project waiting for approval: {project.Name}<br>" +
                "Warm regards,<br>" +
                "The CollAction team";

            var administrators = await _userManager.GetUsersInRoleAsync("admin");
            foreach (var admin in administrators)
                await _emailSender.SendEmailAsync(admin.Email, subject, confirmationEmailAdmin);

            return View("ThankYouCreate", new ThankYouCreateProjectViewModel()
            {
                Name = project.Name
            });
        }

        // GET: Projects/Edit/5
        /*
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                                        .Include(p => p.BannerImage)
                                        .Include(p => p.DescriptionVideoLink)
                                        .Include(p => p.Tags).ThenInclude(t => t.Tag)
                                        .SingleOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(User) != project.OwnerId)
            {
                return Forbid();
            }

            var editProjectViewModel = new EditProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Goal = project.Goal,
                Proposal = project.Proposal,
                CreatorComments = project.CreatorComments,
                CategoryId = project.CategoryId,
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description", project.CategoryId),
                LocationId = project.LocationId,
                Target = project.Target,
                Start = project.Start,
                End = project.End,
                DescriptionVideoLink = project.DescriptionVideoLink?.Link,
                BannerImageFile = project.BannerImage,
                BannerImageDescription = project.BannerImage?.Description,
                Hashtag = project.HashTags
            };

            return View(editProjectViewModel);
        }
        */

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> Edit(int id, EditProjectViewModel editProjectViewModel)
        //{
        //    if (id != editProjectViewModel.Id)
        //    {
        //        return NotFound();
        //    }

        //    var project = await _context.Projects.Include(p => p.BannerImage).Include(p => p.DescriptionVideoLink).SingleOrDefaultAsync(m => m.Id == id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    if (_userManager.GetUserId(User) != project.OwnerId)
        //    {
        //        return Forbid();
        //    }

        //    // If the project name changed make sure it is still unique.
        //    if (project.Name != editProjectViewModel.Name && await _context.Projects.AnyAsync(p => p.Name == editProjectViewModel.Name))
        //    {
        //        ModelState.AddModelError("Name", _localizer["A project with the same name already exists."]);
        //    }

        //    // If there are image descriptions without corresponding image uploads, warn the user.
        //    if (project.BannerImage == null && editProjectViewModel.BannerImageUpload == null && !string.IsNullOrWhiteSpace(editProjectViewModel.BannerImageDescription))
        //    {
        //        ModelState.AddModelError("BannerImageDescription", _localizer["You can only provide a 'Banner Image Description' if you upload a 'Banner Image'."]);
        //    }
        //    if (project.DescriptiveImage == null && editProjectViewModel.DescriptiveImageUpload == null && !string.IsNullOrWhiteSpace(editProjectViewModel.DescriptiveImageDescription))
        //    {
        //        ModelState.AddModelError("DescriptiveImageDescription", _localizer["You can only provide a 'DescriptiveImage Description' if you upload a 'DescriptiveImage'."]);
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        editProjectViewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
        //        editProjectViewModel.BannerImage = project.BannerImage;
        //        editProjectViewModel.DescriptiveImage = project.DescriptiveImage;
        //        return View(editProjectViewModel);
        //    }

        //    project.Name = editProjectViewModel.Name;
        //    project.Description = editProjectViewModel.Description;
        //    project.Goal = editProjectViewModel.Goal;
        //    project.Proposal = editProjectViewModel.Proposal;
        //    project.CreatorComments = editProjectViewModel.CreatorComments;
        //    project.CategoryId = editProjectViewModel.CategoryId;
        //    project.LocationId = editProjectViewModel.LocationId;
        //    project.Target = editProjectViewModel.Target;
        //    project.Start = editProjectViewModel.Start;
        //    project.End = editProjectViewModel.End;

        //    var bannerImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "bannerimages"));
        //    project.BannerImage = await bannerImageManager.CreateOrReplaceImageFileIfNeeded(project.BannerImage, editProjectViewModel.BannerImageUpload, editProjectViewModel.BannerImageDescription);

        //    var descriptiveImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "descriptiveimages"));
        //    project.DescriptiveImage = await descriptiveImageManager.CreateOrReplaceImageFileIfNeeded(project.DescriptiveImage, editProjectViewModel.DescriptiveImageUpload, editProjectViewModel.DescriptiveImageDescription);

        //    project.SetDescriptionVideoLink(_context, editProjectViewModel.DescriptionVideoLink);

        //    await project.SetTags(_context, editProjectViewModel.Hashtag?.Split(';') ?? new string[0]);

        //    try
        //    {
        //        _context.Update(project);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Find");
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!(await _context.Projects.AnyAsync(e => e.Id == id)))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

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

        // POST: Projects/Delete/5
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

            var project =  await _service.GetProjectById(id); 
            if (project == null)
            {
                return NotFound();
            }

            var commitProjectViewModel = new CommitProjectViewModel
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                ProjectProposal = project.Proposal,
                IsUserCommitted = (await _service.GetParticipant((await _userManager.GetUserAsync(User)).Id, project.Id) != null),
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
            bool success = await _service.AddParticipant(user.Id, commitProjectViewModel.ProjectId);

            if (success)
            {
                string confirmationEmail = 
                    "Hi!<br><br>" +
                    "Thank you for participating in a CollAction project!<br><br>" +
                    "In crowdacting, we only act collectively when we meet the target before the deadline, so please feel very welcome to share this project on social media through the social media buttons on the project page!<br><br>" +
                    "We'll keep you updated on the project. Also feel free to Like us on <a href=\"https://www.facebook.com/collaction.org/\">Facebook</a> to stay up to date on everything CollAction!<br><br>" +
                    "Warm regards,<br>The CollAction team";
                string subject = "Thank you for participating in a CollAction project!";
                await _emailSender.SendEmailAsync(user.Email, subject, confirmationEmail);
                return View("ThankYouCommit", commitProjectViewModel);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTileProjects(int? categoryId, int? statusId)
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
            
            var projects = await _service.GetTileProjects(
                Expression.Lambda<Func<Project, bool>>(Expression.AndAlso(projectExpression.Body, Expression.Invoke(statusExpression, projectExpression.Parameters[0])), projectExpression.Parameters[0]));
            
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
