using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CollAction.Models;
using CollAction.ViewModels.Account;
using System.Net;
using CollAction.Services.User;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using System.Linq;

namespace CollAction.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUserService userService;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            IUserService userService,
            ILogger<AccountController> logger)
        {
            this.signInManager = signInManager;
            this.userService = userService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string errorUrl, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                SignInResult result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    logger.LogInformation("User logged in");
                    return Redirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    logger.LogInformation("User is locked out");
                    return Redirect($"{errorUrl}?error=lockout");
                }
                else
                {
                    logger.LogInformation("User is unable to log in");
                    return Redirect($"{errorUrl}?error=other");
                }
            }
            else
            {
                return Redirect($"{errorUrl}?error=validation");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LogOff()
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out.");
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string errorUrl, string returnUrl = null)
        {
            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ErrorUrl = errorUrl, ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string errorUrl, string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                string error = $"Error from external login: {remoteError}";
                logger.LogError(error);
                return Redirect($"{errorUrl}?error={WebUtility.UrlEncode(error)}");
            }

            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                string error = $"Error from external login: unable to retrieve user data";
                logger.LogError(error);
                return Redirect($"{errorUrl}?error={WebUtility.UrlEncode(error)}");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                logger.LogInformation("User logged in with {0} provider.", info.LoginProvider);
                return Redirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogInformation("User is locked out");
                return Redirect($"{errorUrl}?error=lockout");
            }
            else
            {
                // If the user does not have an account, create one
                string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var newUserResult = await userService.CreateUser(email, info);
                if (newUserResult.Result.Succeeded)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    string error = string.Join(", ", newUserResult.Result.Errors.Select(e => e.Description));
                    logger.LogError(error);
                    return Redirect($"{errorUrl}?error={WebUtility.UrlEncode(error)}");
                }
            }
        }
    }
}
