using CollAction.Data;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Models.AdminViewModels;
using CollAction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CollAction.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController: Controller
    {
        public AdminController(
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<AccountController> localizer,
            IHostingEnvironment hostingEnvironment,
            IEmailSender emailSender,
            IProjectService projectService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _localizer = localizer;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
            _projectService = projectService;
        }

        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IProjectService _projectService;

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
                DescriptionVideoLink = project.DescriptionVideoLink?.Link,
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
            string csv = await _projectService.GenerateParticipantsDataExport(id);
            if (csv != null)
                return Content(csv, "text/csv");
            else
                return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDescriptiveImage(int id)
        {
            var fileManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "descriptiveimages"));
            Project project = await _context.Projects.Include(p => p.DescriptiveImage).FirstAsync(p => p.Id == id);
            fileManager.DeleteImageFileIfExists(project.DescriptiveImage);
            project.DescriptiveImageFileId = null;
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageProject", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBannerImage(int id)
        {
            var fileManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "bannerimages"));
            Project project = await _context.Projects.Include(p => p.BannerImage).FirstAsync(p => p.Id == id);
            fileManager.DeleteImageFileIfExists(project.BannerImage);
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
                ModelState.AddModelError("Name", "Een project met deze naam bestaat al. Kies ajb een nieuwe naam.");
            }

            // If there are image descriptions without corresponding image uploads, warn the user.
            if (project.BannerImage == null && model.BannerImageUpload == null && !string.IsNullOrWhiteSpace(model.BannerImageDescription))
            {
                ModelState.AddModelError("BannerImageDescription", "Je kunt deze beschrijving alleen toevoegen als je een banner afbeelding toevoegt");
            }
            if (project.DescriptiveImage == null && model.DescriptiveImageUpload == null && !string.IsNullOrWhiteSpace(model.DescriptiveImageDescription))
            {
                ModelState.AddModelError("DescriptiveImageDescription", "Je kunt deze beschrijving alleen toevoegen als je een afbeelding toevoegt");
            }

            if (ModelState.IsValid)
            {
                bool approved = model.Status == ProjectStatus.Running && project.Status == ProjectStatus.Hidden;
                bool successfull = model.Status == ProjectStatus.Successful && project.Status == ProjectStatus.Running;
                bool failed = model.Status == ProjectStatus.Failed && project.Status == ProjectStatus.Running;

                if (approved)
                {
                    //"Hi!<br>" +
                    //"<br>" +
                    //"The CollAction Team has reviewed your project proposal and is very happy to share that your project has been approved and now live on www.collaction.org!<br>" +
                    //"<br>" +
                    //"So feel very welcome to start promoting it! If you have any further questions, feel free to contact the CollAction Team at collactionteam@gmail.com. And don’t forget to tag CollAction in your messages on social media so we can help you spread the word(FB: @collaction.org, Twitter: @collaction_org)!<br>" +
                    //"<br>" +
                    //"Thanks again for driving the CollAction / crowdacting movement!<br>" +
                    //"<br>" +
                    //"Warm regards,<br>" +
                    //"The CollAction team<br>";

                    string approvalEmail =
                        "Hi!<br>" +
                        "<br>" +
                        "Het Freonen team heeft je projectvoorstel bekeken en goedgekeurd! Het staat nu live op www.freonen.collaction.org<br>" +
                        "<br>" +
                        "Heel veel succes met de promotie van het project! Als er nog vragen zijn, kun je een mailtje sturen naar info@freonen.nl. En vergeet ons niet te taggen op social media zodat we je kunnen helpen het woord te verspreiden (FB: @freonen, Twitter: @freonen)!<br>" +
                        "<br>" +
                        "Dank voor je inzet voor een nog mooiere wereld!<br>" +
                        "<br>" +
                        "Warme groet,<br>" +
                        "Het Freonen team<br>";

                    string subject = $"Approval - {project.Name}";

                    await _emailSender.SendEmailAsync(project.Owner.Email, subject, approvalEmail);
                }
                else if (successfull)
                {
                        //"Hi!<br>" +
                        //"<br>" +
                        //"The deadline of the project you have started on www.collaction.org has passed. We're very happy to see that the target you have set has been reached! Congratulations! Now it's time to act collectively!<br>" +
                        //"<br>" +
                        //"The CollAction Team might reach out to you with more specifics (this is an automated message). If you have any further questions yourself, feel free to contact the CollAction Team at collactionteam@gmail.com. And don’t forget to tag CollAction in your messages on social media so we can help you spread the word on your achievement (FB: @collaction.org, Twitter: @collaction_org)!<br>" +
                        //"<br>" +
                        //"Thanks again for driving the CollAction / crowdacting movement!<br>" +
                        //"<br>" +
                        //"Warm regards,<br>" +
                        //"The CollAction team<br>";
                    string successEmail =
                        "Hi!<br>" +
                        "<br>" +
                        "De deadline voor je project op freonen.collaction.org is verstreken en we zijn blij om te zien dat je target is gehaald. Gefeliciteerd! Het is nu dus tijd om samen in actie te komen!<br>" +
                        "<br>" +
                        "Het Freonen team komt misschien nog bij je terug om de details te bespreken (dit is een automatisch bericht), maar als je in de tussentijd nog vragen hebt, neem dan vooral contact op via info @freonen.nl.En vergeet ons niet te taggen op social media zodat we je kunnen helpen het woord te verspreiden(FB: @freonen, Twitter: @freonen)!<br>" +
                        "<br>" +
                        "Dank voor je inzet voor een nog mooiere en duurzamere wereld!<br>" +
                        "<br>" +
                        "Warme groet,<br>" +
                        "Het Freonen team<br>";

                    string subject = $"Success - {project.Name}";

                    await _emailSender.SendEmailAsync(project.Owner.Email, subject, successEmail);
                }
                else if (failed)
                {
                        //"Hi!<br>" +
                        //"<br>" +
                        //"The deadline of the project you have started on www.collaction.org has passed. Unfortunately the target that you have set has not been reached. Great effort though!<br>" +
                        //"<br>" +
                        //"The CollAction Team might reach out to you with more specifics (this is an automated message). If you have any further questions yourself, feel free to contact the CollAction Team at collactionteam@gmail.com.<br>" +
                        //"<br>" +
                        //"Thanks again for driving the CollAction / crowdacting movement and better luck next time!<br>" +
                        //"<br>" +
                        //"Warm regards,<br>" +
                        //"The CollAction team<br>";
                    string failedEmail =
                        "Hi!<br>" +
                        "<br>" +
                        "De deadline voor je project op freonen.collaction.org is verstreken. Helaas is het target dat je hebt gesteld niet gehaald. Dit betekent volgens het crowdacting concept dat de deelnemers niet in actie hoeven te komen (maar dat mag natuurlijk wel!)<br>" +
                        "<br>" +
                        "Het Freonen team komt misschien nog bij je terug om de details te bespreken (dit is een automatisch bericht), maar als je in de tussentijd nog vragen hebt, neem dan vooral contact op via info @freonen.nl.<br>" +
                        "<br>" +
                        "Dank voor je inzet voor een nog mooiere en duurzamere wereld!<br>" +
                        "<br>" +
                        "Warme groet,<br>" +
                        "Het Freonen team<br>"; ;

                    string subject = $"Failed - {project.Name}";

                    await _emailSender.SendEmailAsync(project.Owner.Email, subject, failedEmail);
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

                var bannerImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "bannerimages"));
                project.BannerImage = await bannerImageManager.CreateOrReplaceImageFileIfNeeded(project.BannerImage, model.BannerImageUpload, model.BannerImageDescription);

                var descriptiveImageManager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "descriptiveimages"));
                project.DescriptiveImage = await descriptiveImageManager.CreateOrReplaceImageFileIfNeeded(project.DescriptiveImage, model.DescriptiveImageUpload, model.DescriptiveImageDescription);

                await project.SetTags(_context, model.Hashtag?.Split(';') ?? new string[0]);

                project.SetDescriptionVideoLink(_context, model.DescriptionVideoLink);

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
