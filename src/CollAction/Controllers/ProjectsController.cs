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
using System.Text.RegularExpressions;

namespace CollAction.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<ProjectsController> _localizer;

        public ProjectsController(ApplicationDbContext context, IStringLocalizer<ProjectsController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public ViewResult StartInfo()
        {
            return View();
        }

        [Authorize]
        public ViewResult StartForm()
        {
            return View("Create");
        }

        // GET: Project/Find
        public async Task<IActionResult> Find(FindProjectViewModel model)
        {
            if (model.SearchText == null) {
                return View(new FindProjectViewModel { Projects = new List<Project>() });
            }

            // TODO: Deal with different languages! So far only english handled, also migrations will be needed for other languages too.

            string[] searchTerms = GetValidSearchTerms(model.SearchText);
            model.Projects = new List<Project>();
            if (searchTerms.Length != 0) {
                string language = "english";
                model.Projects = await BuildFullTextSearchQuery<Project>(_context.Project, searchTerms, language).ToListAsync();
            }
            
            return View(model);
        }

        private string[] GetValidSearchTerms(string searchText)
        {
            string[] searchTerms = { };
            if (searchText.Length != 0)
            {
                string pattern = "\\W+"; // Match any non-word characters
                string replacement = " ";
                Regex regex = new Regex(pattern);
                string justWordCharactersAndSpaces = regex.Replace(searchText, replacement);
                searchTerms = justWordCharactersAndSpaces.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return searchTerms;
        }

        private IQueryable<T> BuildFullTextSearchQuery<T>(DbSet<T> entitySet, string[] searchTerms, string language) where T : class
        {
            string entityName =     typeof(T).Name;
            string tsvectorTerm =   GetTsvectorTerm(language);
            string tsrankTerm =     GetTsrankTerm(tsvectorTerm);
            string tsqueryTerm =    GetTsqueryTermForSearchParameter(language);
            string searchTerm =     String.Join("|", searchTerms);
            return entitySet.FromSql(
                        "SELECT *, " + tsrankTerm + " " +
                        "FROM \"" + entityName + "\", " + tsqueryTerm + " \"Query\" " +
                        "WHERE \"Query\" @@ " + tsvectorTerm + " " +
                        "ORDER BY \"Rank\" DESC LIMIT 20 ",
                        searchTerm);
        }

        private string GetTsvectorTerm(string language)
        {
            return "\"FullTextSearchVector_" + language + "\"";
        }

        private string GetTsrankTerm(string tsvectorTerm)
        {
            return "ts_rank_cd(" + tsvectorTerm + ", \"Query\") AS \"Rank\"";
        }

        private string GetTsqueryTermForSearchParameter(string language)
        {
            return "to_tsquery('" + language + "', @p0)";
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Project.Include(p => p.Owner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.SingleOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Deadline,Description,Name,OwnerId,ShortDescription,Target,Title")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", project.OwnerId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.SingleOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", project.OwnerId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Deadline,Description,Name,OwnerId,ShortDescription,Target,Title")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
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
                    if (!ProjectExists(project.Id))
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", project.OwnerId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.SingleOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.SingleOrDefaultAsync(m => m.Id == id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
