using CollAction.Data;
using CollAction.Models;
using CollAction.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CollAction.Services.Project;
using CollAction.Services.Email;
using CollAction.Services.Image;

namespace CollAction.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class AdminController: Controller
    {
        public AdminController(
            IEmailSender emailSender,
            IParticipantsService participantsService,
            IImageService imageService,
            ApplicationDbContext context)
        {
            _context = context;
            _emailSender = emailSender;
            _participantsService = participantsService;
            _imageService = imageService;
        }

        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IParticipantsService _participantsService;
        private readonly IImageService _imageService;

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpGet]
        public async Task<IActionResult> ManageUsersIndex(int page = 0)
        {
            if (page < 0)
                throw new ApplicationException($"invalid page size: {page}");

            const int pageSize = 20;

            ManageUsersIndexViewModel model = new ManageUsersIndexViewModel()
            {
                Users = await _context.Users.Skip(pageSize * page).Take(pageSize).ToListAsync(),
                NumberPages = 1 + await _context.Users.CountAsync() / pageSize,
                CurrentPage = page
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUser(string userId)
        {
            ApplicationUser user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new ApplicationException($"unable to find user: {userId}");

            ManageUserViewModel model = new ManageUserViewModel()
            {
                Id = userId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RepresentsNumberParticipants = user.RepresentsNumberParticipants
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUser(ManageUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _context.Users.FindAsync(model.Id);

                if (user == null)
                    throw new ApplicationException($"unable to find user: {model.Id}");

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.RepresentsNumberParticipants = model.RepresentsNumberParticipants;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageUsersIndex));
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageProjectsIndex()
            => View(await _context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> ManageProject(int id)
        {
            Project project = await _context.Projects.Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
                .Include(p => p.DescriptionVideoLink)
                .Include(p => p.BannerImage)
                .Include(p => p.DescriptiveImage)
                .FirstOrDefaultAsync(p => p.Id == id);

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
                DescriptionVideoLink = project.DescriptionVideoLink,
                BannerImageFile = project.BannerImage,
                BannerImageDescription = project.BannerImage?.Description,
                DescriptiveImageFile = project.DescriptiveImage,
                DescriptiveImageDescription = project.DescriptiveImage?.Description,
                End = project.End,
                Start = project.Start,
                Target = project.Target,
                DisplayPriority = project.DisplayPriority,
                Goal = project.Goal,
                OwnerId = project.OwnerId,
                Status = project.Status,
                Proposal = project.Proposal,
                Id = project.Id
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ParticipantsDataExport(int id)
        {
            string csv = await _participantsService.GenerateParticipantsDataExport(id);
            if (csv != null)
                return Content(csv, "text/csv");
            else
                return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDescriptiveImage(int id)
        {
            Project project = await _context.Projects.Include(p => p.DescriptiveImage).FirstAsync(p => p.Id == id);
            _context.ImageFiles.Remove(project.DescriptiveImage);
            _imageService.DeleteImage(project.DescriptiveImage);
            project.DescriptiveImageFileId = null;
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageProject", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBannerImage(int id)
        {
            Project project = await _context.Projects.Include(p => p.BannerImage).FirstAsync(p => p.Id == id);
            _context.ImageFiles.Remove(project.BannerImage);
            _imageService.DeleteImage(project.BannerImage);
            project.BannerImageFileId = null;
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageProject", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageProject(ManageProjectViewModel model)
        {
            Project project = await _context.Projects
                .Where(p => p.Id == model.Id)
                .Include(p => p.Owner)
                .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
                .Include(p => p.DescriptionVideoLink)
                .Include(p => p.BannerImage)
                .Include(p => p.DescriptiveImage)
                .FirstAsync();

            // If the project name changed make sure it is still unique.
            if (project.Name != model.Name && await _context.Projects.AnyAsync(p => p.Name == model.Name))
            {
                ModelState.AddModelError("Name", "A project with the same name already exists.");
            }

            // If there are image descriptions without corresponding image uploads, warn the user.
            if (project.BannerImage == null && model.BannerImageUpload == null && !string.IsNullOrWhiteSpace(model.BannerImageDescription))
            {
                ModelState.AddModelError("BannerImageDescription", "You can only provide a 'Banner Image Description' if you upload a 'Banner Image'.");
            }
            if (project.DescriptiveImage == null && model.DescriptiveImageUpload == null && !string.IsNullOrWhiteSpace(model.DescriptiveImageDescription))
            {
                ModelState.AddModelError("DescriptiveImageDescription", "You can only provide a 'DescriptiveImage Description' if you upload a 'DescriptiveImage'.");
            }

            if (ModelState.IsValid)
            {
                if (project.Owner != null)
                {
                    bool approved = model.Status == ProjectStatus.Running && project.Status == ProjectStatus.Hidden;
                    bool successfull = model.Status == ProjectStatus.Successful && project.Status == ProjectStatus.Running;
                    bool failed = model.Status == ProjectStatus.Failed && project.Status == ProjectStatus.Running;

                    if (approved)
                    {
                        await _emailSender.SendEmailTemplated(project.Owner.Email, $"Approval - {project.Name}", "ProjectApproval");
                    }
                    else if (successfull)
                    {
                        await _emailSender.SendEmailTemplated(project.Owner.Email, $"Success - {project.Name}", "ProjectSuccess");
                    }
                    else if (failed)
                    {
                        await _emailSender.SendEmailTemplated(project.Owner.Email, $"Failed - {project.Name}", "ProjectFailed");
                    }
                }

                project.Name = model.Name;
                project.Description = model.Description;
                project.Proposal = model.Proposal;
                project.Goal = model.Goal;
                project.CreatorComments = model.CreatorComments;
                project.CategoryId = model.CategoryId;
                project.Target = model.Target;
                project.Start = model.Start;
                project.End = model.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                project.Status = model.Status;
                project.OwnerId = model.OwnerId;
                project.DisplayPriority = model.DisplayPriority;

                if (model.BannerImageUpload != null)
                {
                    ImageFile uploaded = await _imageService.UploadImage(project.BannerImage, model.BannerImageUpload, model.BannerImageDescription ?? string.Empty);
                    if (project.BannerImage == null)
                    {
                        project.BannerImage = uploaded;
                        _context.ImageFiles.Add(project.BannerImage);
                    }
                }
                else if (model.BannerImageDescription != project.BannerImage?.Description && project.BannerImage != null)
                {
                    project.BannerImage.Description = model.BannerImageDescription ?? string.Empty;
                }

                if (model.DescriptiveImageUpload != null)
                {
                    ImageFile uploaded = await _imageService.UploadImage(project.DescriptiveImage, model.DescriptiveImageUpload, model.DescriptiveImageDescription ?? string.Empty);
                    if (project.DescriptiveImage == null)
                    {
                        project.DescriptiveImage = uploaded;
                        _context.ImageFiles.Add(project.DescriptiveImage);
                    }
                }
                else if (model.DescriptiveImageDescription != project.DescriptiveImage?.Description && project.DescriptiveImage != null)
                {
                    project.DescriptiveImage.Description = model.DescriptiveImageDescription ?? string.Empty;
                }

                project.DescriptionVideoLink = model.DescriptionVideoLink;

                await _context.SaveChangesAsync();
                return RedirectToAction("ManageProjectsIndex");
            }
            else
            {
                model.UserList = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", null);
                model.CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", null);
                model.DisplayPriorityList = new SelectList(Enum.GetValues(typeof(ProjectDisplayPriority)));
                model.StatusList = new SelectList(Enum.GetValues(typeof(ProjectStatus)));
                model.BannerImageFile = project.BannerImage;
                model.DescriptiveImageFile = project.DescriptiveImage;
                return View(model);
            }
        }
    }
}
