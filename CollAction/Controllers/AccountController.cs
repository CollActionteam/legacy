using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.User;
using CollAction.Services.User.Models;
using CollAction.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
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
                return Redirect($"{model.ErrorUrl}?error=validation&message={WebUtility.UrlEncode(ModelState.GetValidationString())}&returnUrl={model.ReturnUrl}");
            }

            SignInResult result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true).ConfigureAwait(false);
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
                    return Redirect($"{model.ErrorUrl}?error=lockout&message={WebUtility.UrlEncode("User is locked out")}&returnUrl={model.ReturnUrl}");
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
                    return Redirect($"{model.ErrorUrl}?error=invalid-credentials&message={WebUtility.UrlEncode("Invalid credentials")}&returnUrl={model.ReturnUrl}");
                }
                else
                {
                    return Unauthorized("invalid-credentials");
                }
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout(LogoutViewModel logoutViewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException(nameof(LogoutViewModel));
            }

            await signInManager.SignOutAsync().ConfigureAwait(false);
            logger.LogInformation("User logged out.");
            return Redirect(logoutViewModel.ReturnUrl);
        }

        [HttpPost]
        public IActionResult ExternalLogin(ExternalLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState.GetValidationString());
            }

            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { model.ErrorUrl, model.ReturnUrl, model.FinishRegistrationUrl, model.RememberMe });
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
                return Redirect($"{model.ErrorUrl}?error=external-login&message={WebUtility.UrlEncode(error)}&returnUrl={model.ReturnUrl}");
            }

            ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
            if (info == null)
            {
                string error = $"Error from external login: unable to retrieve user data";
                logger.LogError(error);
                return Redirect($"{model.ErrorUrl}?error=external-userdata&message={WebUtility.UrlEncode(error)}&returnUrl={model.ReturnUrl}");
            }

            // Sign in the user with this external login provider if the user already has a login.
            SignInResult result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false).ConfigureAwait(false);
            if (result.Succeeded)
            {
                logger.LogInformation("User logged in with {0} provider.", info.LoginProvider);
                return Redirect(model.ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogInformation("User is locked out");
                return Redirect($"{model.ErrorUrl}?error=lockout&message={WebUtility.UrlEncode("User is locked out")}&returnUrl={model.ReturnUrl}");
            }

            // If the user can't login with the external login, 
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ExternalUserResult newUserResult = await userService.CreateOrAddExternalToUser(email, info).ConfigureAwait(false);
            if (newUserResult.Result.Succeeded)
            {
                if (newUserResult.AddedUser)
                {
                    return Redirect(model.FinishRegistrationUrl);
                }
                else
                {
                    return Redirect(model.ReturnUrl);
                }
            }
            else
            {
                string error = string.Join(", ", newUserResult.Result.Errors.Select(e => e.Description));
                logger.LogError(error);
                return Redirect($"{model.ErrorUrl}?error=external-login-create&message={WebUtility.UrlEncode(error)}&returnUrl={model.ReturnUrl}");
            }
        }
    }
}
