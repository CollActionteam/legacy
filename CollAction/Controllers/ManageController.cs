using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CollAction.Models;
using CollAction.Models.ManageViewModels;
using CollAction.Services.Newsletter;
using CollAction.Services.Project;
using CollAction.Services.Donation;
using System.Linq;

namespace CollAction.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDonationService _donationService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INewsletterSubscriptionService _newsletterSubscriptionService;
        private readonly ILogger _logger;
        private readonly IProjectService  _projectService;

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          INewsletterSubscriptionService newsletterSubscriptionService,
          ILoggerFactory loggerFactory,
          IDonationService donationService,
          IProjectService projectService)
        {
            _userManager = userManager;
            _donationService = donationService;
            _signInManager = signInManager;
            _newsletterSubscriptionService = newsletterSubscriptionService;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                NewsletterSubscription = await _newsletterSubscriptionService.IsSubscribedAsync(user.Email),
                DonationSubscriptions = (await _donationService.GetSubscriptionsFor(user)).Where(s => !s.CanceledAt.HasValue)
            };
            return View(model);
        }

        [HttpGet]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MyProjects()
        {
            var user = await GetCurrentUserAsync();
            
            var projects = await _projectService.MyProjects(user.Id);
            return Json(projects);
        }

        [HttpGet]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ParticipatingInProjects()
        {
            var user = await GetCurrentUserAsync();
            
            var projects = await _projectService.ParticipatingInProjects(user.Id);
            return Json(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsletterSubscription([Bind("NewsletterSubscription")]IndexViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _newsletterSubscriptionService.SetSubscription(user.Email, model.NewsletterSubscription, false);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEmailSubscription(int projectId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            await _projectService.ToggleNewsletterSubscription(projectId, user.Id);
            return RedirectToAction(nameof(Index));
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
