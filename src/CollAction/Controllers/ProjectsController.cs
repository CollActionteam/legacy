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
        public async Task<IActionResult> Find(FindProjectsViewModel model)
        {
            if (model.SearchText == null)
            {
                return View(new FindProjectsViewModel
                {
                    OwnerId = (await _userManager.GetUserAsync(User)).Id,
                    Projects = await DisplayProjectViewModel.GetViewModelsWhere(_context, p => p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted)
                });
            }

            model.OwnerId = (await _userManager.GetUserAsync(User)).Id;
            model.Projects = await DisplayProjectViewModel.GetViewModelsWhere(_context, p => p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted &&
                (p.Name.Contains(model.SearchText) || p.Description.Contains(model.SearchText) || p.Goal.Contains(model.SearchText)));
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            return View(new BrowseProjectsViewModel
            {
                OwnerId = (await _userManager.GetUserAsync(User)).Id,
                Projects = await DisplayProjectViewModel.GetViewModelsWhere(_context, p => p.Status != ProjectStatus.Hidden && p.Status != ProjectStatus.Deleted)
            });
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

            return View(items.First());
        }

        // GET: Projects/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View(new CreateProjectViewModel
            {
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description"),
                Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null)
            });
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Name,Description,Goal,CategoryId,LocationId,Target,End")] CreateProjectViewModel createProjectViewModel)
        {
            // Make sure the project name is unique.
            if (await _context.Projects.AnyAsync(p => p.Name == createProjectViewModel.Name))
            {
                ModelState.AddModelError("Name", _localizer["A project with the same name already exists."]);
            }

            if (!ModelState.IsValid) {
                createProjectViewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
                createProjectViewModel.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null);
                return View(createProjectViewModel);
            }

            var project = new Project
            {
                OwnerId = (await _userManager.GetUserAsync(User)).Id,
                Name = createProjectViewModel.Name,
                Description = createProjectViewModel.Description,
                Goal = createProjectViewModel.Goal,
                CategoryId = createProjectViewModel.CategoryId,
                LocationId = createProjectViewModel.LocationId,
                Target = createProjectViewModel.Target,
                End = createProjectViewModel.End,
                Start = DateTime.UtcNow
            };
            
            _context.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
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

            var editProjectViewModel = new EditProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Goal = project.Goal,
                CategoryId = project.CategoryId,
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description", project.CategoryId),
                LocationId = project.LocationId,
                Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", project.LocationId),
                Target = project.Target,
                End = project.End
            };

            return View(editProjectViewModel);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Goal,CategoryId,LocationId,Target,End")] EditProjectViewModel editProjectViewModel)
        {
            if (id != editProjectViewModel.Id)
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

            // If the project name changed make sure it is still unique.
            if (project.Name != editProjectViewModel.Name && await _context.Projects.AnyAsync(p => p.Name == editProjectViewModel.Name))
            {
                ModelState.AddModelError("Name", _localizer["A project with the same name already exists."]);
            }

            if (!ModelState.IsValid)
            {
                editProjectViewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
                editProjectViewModel.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null);
                return View(editProjectViewModel);
            }

            project.Name = editProjectViewModel.Name;
            project.Description = editProjectViewModel.Description;
            project.Goal = editProjectViewModel.Goal;
            project.CategoryId = editProjectViewModel.CategoryId;
            project.LocationId = editProjectViewModel.LocationId;
            project.Target = editProjectViewModel.Target;
            project.End = editProjectViewModel.End;

            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
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
