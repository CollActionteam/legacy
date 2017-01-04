using CollAction.Data;
using CollAction.Models;
using CollAction.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController: Controller
    {
        public AdminController(
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<AccountController> localizer,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _localizer = localizer;
            _context = context;
        }

        private readonly ApplicationDbContext _context;
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
                Project = project,
                UserList = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", null),
                LocationList = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null),
                CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", null),
                DisplayPriorityList = new SelectList(Enum.GetValues(typeof(ProjectDisplayPriority))),
                StatusList = new SelectList(Enum.GetValues(typeof(ProjectStatus))),
                ProjectTags = project.HashTags
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageProject([Bind("Project,ProjectTags")]ManageProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model.Project);
                    await _context.SaveChangesAsync();
                    await model.Project.SetTags(_context, model.ProjectTags?.Split(';') ?? new string[0]);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _context.Projects.AnyAsync(e => e.Id == model.Project.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ManageProjectsIndex");
            }
            return await ManageProject(model.Project.Id);
        }
    }
}
