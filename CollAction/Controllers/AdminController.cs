using CollAction.Data;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController: Controller
    {
        public AdminController(
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<AccountController> localizer,
            IHostingEnvironment hostingEnvironment,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _localizer = localizer;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpGet]
        public async Task<IActionResult> ManageProjectsIndex()
            => View(await _context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> ManageProject(int id)
        {
            Project project = await _context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
                return NotFound();

            ManageProjectViewModel model = new ManageProjectViewModel()
            {
                UserList = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", null),
                CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", null),
                DisplayPriorityList = new SelectList(Enum.GetValues(typeof(ProjectDisplayPriority))),
                StatusList = new SelectList(Enum.GetValues(typeof(ProjectStatus))),
                Hashtag = project.HashTags,
                Name = project.Name,
                Description = project.Description,
                CategoryId = project.CategoryId,
                CreatorComments = project.CreatorComments,
                End = project.End,
                Start = project.Start,
                Target = project.Target,
                DisplayPriority = project.DisplayPriority,
                Goal = project.Goal,
                OwnerId = project.OwnerId,
                Status = project.Status,
                Proposal = project.Proposal
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageProject(ManageProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Project project = await _context.Projects.FindAsync(model.Id);
                project.Name = model.Name;
                project.Description = model.Description;
                project.Goal = model.Goal;
                project.Proposal = model.Proposal;
                project.CreatorComments = model.CreatorComments;
                project.CategoryId = model.CategoryId;
                project.Target = model.Target;
                project.Start = model.Start;
                project.End = model.End;
                project.Status = model.Status;
                project.OwnerId = model.OwnerId;
                project.DisplayPriority = model.DisplayPriority;
                if (model.HasBannerImageUpload)
                {
                    var manager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "bannerimages"));
                    if (project.BannerImage != null)
                    {
                        manager.DeleteImageFile(project.BannerImage);
                    }
                    project.BannerImage = await manager.UploadFormFile(model.BannerImageUpload, Guid.NewGuid().ToString() /* unique filename */);
                }
                await project.SetTags(_context, model.Hashtag?.Split(';') ?? new string[0]);
                project.SetDescriptionVideoLink(_context, model.DescriptionVideoLink);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageProjectsIndex");
            }
            else
                return View(model);
        }
    }
}
