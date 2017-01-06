using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            if (model.SearchText == null) {
                return View(new FindProjectsViewModel {
                    Projects = await GetProjectDisplayItemsWhere(p => p.Status != ProjectStatus.Hidden)
                });
            }

            model.Projects = await GetProjectDisplayItemsWhere(
                p => p.Status != ProjectStatus.Hidden && (p.Name.Contains(model.SearchText) || p.Description.Contains(model.SearchText) || p.Goal.Contains(model.SearchText)));
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            return View(await GetProjectDisplayItemsWhere(p => p.Status != ProjectStatus.Hidden));
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<DisplayProjectViewModel> items = await GetProjectDisplayItemsWhere(p => p.Id == id && p.Status != ProjectStatus.Hidden);
            if (items.Count == 0)
            {
                return NotFound();
            }

            return View(items.First());
        }

        public async Task<List<DisplayProjectViewModel>> GetProjectDisplayItemsWhere(Expression<Func<Project, bool>> WhereExpression)
        {
            return await _context.Projects
                .Where(WhereExpression)
                .Include(p => p.Category)
                .Include(p => p.Location)
                .GroupJoin(_context.ProjectParticipants,
                    project => project.Id,
                    participants => participants.ProjectId,
                    (project, participantsGroup) => new DisplayProjectViewModel
                    {
                        Project = project,
                        Participants = participantsGroup.Count()
                    })
                .ToListAsync();
        }

        // GET: Projects/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
            var locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null);
            var createProjectViewModel = new EditProjectViewModel(categories, locations);
            return View(createProjectViewModel);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Name,Description,Goal,CategoryId,LocationId,Target,End")] EditProjectViewModel createProjectViewModel)
        {
            ModelState.Clear();
            if (TryValidateModel(createProjectViewModel))
            {
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
                
                try
                {
                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    if (!HandleUpdateException(ex))
                    {
                        throw;
                    }
                }
            }

            createProjectViewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description");
            createProjectViewModel.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", null);
            return View(createProjectViewModel);
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

            ModelState.Clear();
            if (TryValidateModel(editProjectViewModel))
            {
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
                catch (DbUpdateException ex)
                {
                    if (!HandleUpdateException(ex))
                    {
                        throw;
                    }
                }
            }

            editProjectViewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Description", project.CategoryId);
            editProjectViewModel.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", project.LocationId);
            return View(editProjectViewModel);
        }

        private bool HandleUpdateException(Exception ex)
        {
            if (ex.InnerException is PostgresException)
            {
                var pgex = (PostgresException)ex.InnerException;
                if (pgex.SqlState == "23505" && pgex.ConstraintName == "AK_Projects_Name")
                {
                    ModelState.AddModelError(string.Empty, _localizer["A project with the same name already exists."]);
                }
                return true;
            }
            return false;
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
