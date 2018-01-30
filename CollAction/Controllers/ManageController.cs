using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CollAction.Models;
using CollAction.Services;
using Microsoft.Extensions.Localization;
using CollAction.Data;
using CollAction.Models.ManageViewModels;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;

namespace CollAction.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly INewsletterSubscriptionService _newsletterSubscriptionService;
        private readonly ILogger _logger;
        private readonly IStringLocalizer<ManageController> _localizer;
        private readonly ApplicationDbContext _context;

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ISmsSender smsSender,
          INewsletterSubscriptionService newsletterSubscriptionService,
          ILoggerFactory loggerFactory,
          IStringLocalizer<ManageController> localizer,
          ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _newsletterSubscriptionService = newsletterSubscriptionService;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _localizer = localizer;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        //
        // GET: /Manage/Index
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
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage,
                NewsletterSubscription = await _newsletterSubscriptionService.IsSubscribedAsync(user.Email)
            };
            return View(model);
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

            StatusMessage = _localizer["Your profile has been updated"];
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
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

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = Url.Action(action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { user.Id, code },
                protocol: Request.Scheme);
                
            string email = user.Email;
            await _emailSender.SendEmailAsync(
                email,
                _localizer["Confirm your account"],
                $"{_localizer["Please confirm your account by clicking this link"]}: <a href='{callbackUrl}'>{_localizer["link"]}</a>");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        //
        // POST: /Manage/NewsletterSubscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsletterSubscription(IndexViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                try
                {
                    await _newsletterSubscriptionService.SetSubscriptionAsync(user.Email, model.NewsletterSubscription, false);
                    StatusMessage = _localizer["Successfully subscribed to newsletter"];
                }
                catch (Exception e)
                {
                    _logger.LogError("error subscribing to newsletter: {0}, {1}", e, user.Email);
                }
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
