using System;
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

namespace CollAction.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<ProjectsController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, IStringLocalizer<ProjectsController> localizer, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
        }

        public ViewResult StartInfo()
        {
            return View();
        }

        // GET: Project/Find
        public async Task<IActionResult> Find(FindProjectViewModel model)
        {
            if (model.SearchText == null) {
                return View(new FindProjectViewModel { Projects = await _context.Projects.ToListAsync() });
            }

            model.Projects = await _context.Projects.Where(p => p.Name.Contains(model.SearchText) || p.Description.Contains(model.SearchText) || p.Goal.Contains(model.SearchText)).ToListAsync();
            
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Owner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(project);
        }

        // GET: Projects/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
            var locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null);
            var createProjectViewModel = new CreateProjectViewModel(categories, locations);
            return View(createProjectViewModel);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Name,Description,Goal,CategoryId,LocationId,Target,End")] CreateProjectViewModel createProjectVM)
        {
            ModelState.Clear();
            if (TryValidateModel(createProjectVM))
            {
                var project = new Project
                {
                    OwnerId = (await _userManager.GetUserAsync(User)).Id,
                    Name = createProjectVM.Name,
                    Description = createProjectVM.Description,
                    Goal = createProjectVM.Goal,
                    CategoryId = createProjectVM.CategoryId,
                    LocationId = createProjectVM.LocationId,
                    Target = createProjectVM.Target,
                    End = createProjectVM.End,
                    Start = DateTime.UtcNow
                };
                
                try
                {
                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is PostgresException)
                    {
                        var pgex = (PostgresException)ex.InnerException;
                        if (pgex.SqlState == "23505" && pgex.ConstraintName == "AK_Projects_Name")
                        {
                            ModelState.AddModelError(string.Empty, _localizer["A project with the same name already exists."]);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            createProjectVM.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
            createProjectVM.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null);
            return View(createProjectVM);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
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

            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
            ViewData["LocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Goal,CategoryId,LocationId,Target,End")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(User) != project.OwnerId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _context.Projects.AnyAsync(e => e.Id == id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(project);
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
            return RedirectToAction("Index");
        }
    }
}
