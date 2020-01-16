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
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using CollAction.Services.User.Models;
using System.ComponentModel.DataAnnotations;
using CollAction.Helpers;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CollAction.Controllers
{
    public sealed class AccountController : Controller
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect($"{model.ErrorUrl}?error=validation&message={WebUtility.UrlEncode(ModelState.GetValidationString())}");
            }

            SignInResult result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                logger.LogInformation("User logged in");
                if (model.ReturnUrl != null)
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return Ok();
                }
            }
            else if (result.IsLockedOut)
            {
                logger.LogInformation("User is locked out");
                if (model.ErrorUrl != null)
                {
                    return Redirect($"{model.ErrorUrl}?error=lockout");
                }
                else
                {
                    return Unauthorized("lockout");
                }
            }
            else
            {
                logger.LogInformation("User is unable to log in");
                if (model.ErrorUrl != null)
                {
                    return Redirect($"{model.ErrorUrl}?error=invalid-credentials");
                }
                else
                {
                    return Unauthorized("invalid-credentials");
                }
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
        public IActionResult ExternalLogin(ExternalLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState.GetValidationString());
            }

            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { model.ErrorUrl, model.ReturnUrl, model.RememberMe });
            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(model.Provider, redirectUrl);
            return Challenge(properties, model.Provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(ExternalLoginCallbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState.GetValidationString());
            }

            if (model.RemoteError != null)
            {
                string error = $"Error from external login: {model.RemoteError}";
                logger.LogError(error);
                return Redirect($"{model.ErrorUrl}?error={WebUtility.UrlEncode(error)}");
            }

            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                string error = $"Error from external login: unable to retrieve user data";
                logger.LogError(error);
                return Redirect($"{model.ErrorUrl}?error={WebUtility.UrlEncode(error)}");
            }

            // Sign in the user with this external login provider if the user already has a login.
            SignInResult result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                logger.LogInformation("User logged in with {0} provider.", info.LoginProvider);
                return Redirect(model.ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogInformation("User is locked out");
                return Redirect($"{model.ErrorUrl}?error=lockout");
            }

            // If the user does not have an account, create one
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            UserResult newUserResult = await userService.CreateUser(email, info);
            if (newUserResult.Result.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                string error = string.Join(", ", newUserResult.Result.Errors.Select(e => e.Description));
                logger.LogError(error);
                return Redirect($"{model.ErrorUrl}?error={WebUtility.UrlEncode(error)}");
            }
        }
    }
}
