using CollAction.Data;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Email;
using CollAction.Services.Newsletter;
using CollAction.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.User
{
    public sealed class UserService : IUserService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly INewsletterService newsletterService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly ILogger<UserService> logger;
        private readonly ApplicationDbContext context;
        private readonly SiteOptions siteOptions;

        public UserService(
            IServiceProvider serviceProvider,
            INewsletterService newsletterService,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ILogger<UserService> logger,
            ApplicationDbContext context,
            IOptions<SiteOptions> siteOptions)
        {
            this.serviceProvider = serviceProvider;
            this.newsletterService = newsletterService;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.logger = logger;
            this.context = context;
            this.siteOptions = siteOptions.Value;
        }

        public async Task<ExternalUserResult> CreateOrAddExternalToUser(string email, ExternalLoginInfo info)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user != null)
            {
                if (await userManager.IsInRoleAsync(user, AuthorizationConstants.AdminRole).ConfigureAwait(false))
                {
                    // Don't link accounts for admin users, security issue.. 
                    logger.LogError("Attempt to link accounts for admin user: {0}, {1}", email, info.LoginProvider);
                    throw new InvalidOperationException("Attempted to link account for admin user");
                }
                IdentityResult result = await userManager.AddLoginAsync(user, info).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    logger.LogInformation("Added external login to account: {0}, {1}", email, info.LoginProvider);
                }
                else
                {
                    LogErrors("Adding external login", result);
                }

                return new ExternalUserResult(user, result, false);
            }
            else
            {
                logger.LogInformation("Creating user from external login");
                user = new ApplicationUser(email: email, registrationDate: DateTime.UtcNow);
                IdentityResult result = await userManager.CreateAsync(user).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info).ConfigureAwait(false);
                    if (!result.Succeeded)
                    {
                        LogErrors("Adding external login", result);
                    }
                    else
                    {
                        await emailSender.SendEmailTemplated(user.Email, "Account Creation", "UserCreated").ConfigureAwait(false);
                        logger.LogInformation("Created user from external login");
                    }

                    return new ExternalUserResult(user, result, true);
                }
                else
                {
                    LogErrors("Creating user", result);
                    return new ExternalUserResult(result);
                }
            }
        }

        public async Task<UserResult> CreateUser(NewUser newUser)
        {
            IEnumerable<IdentityError> validationResults = ValidationHelper.ValidateAsIdentity(newUser, serviceProvider);
            if (validationResults.Any())
            {
                return new UserResult(IdentityResult.Failed(validationResults.ToArray()));
            }

            ApplicationUser user = new ApplicationUser(email: newUser.Email, firstName: newUser.FirstName, lastName: newUser.LastName, registrationDate: DateTime.UtcNow);
            IdentityResult result = await userManager.CreateAsync(user, newUser.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                newsletterService.SetSubscriptionBackground(newUser.Email, newUser.IsSubscribedNewsletter);
                await emailSender.SendEmailTemplated(user.Email, "Account Creation", "UserCreated").ConfigureAwait(false);
                return new UserResult(user, result);
            }
            else
            {
                LogErrors("Creating user", result);
                return new UserResult(result);
            }
        }

        public async Task<(IdentityResult Result, string? ResetCode)> ForgotPassword(string email)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            {
                var result = IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" });
                LogErrors("Forgot password", result);
                return (result, null);
            }

            logger.LogInformation("Sending reset password for user");
            string code = await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            Uri callbackUrl = new Uri(siteOptions.PublicUrl, $"/account/reset-password?code={WebUtility.UrlEncode(code)}&email={WebUtility.UrlEncode(email)}");
            await emailSender.SendEmailTemplated(email, "Reset Password", "ResetPassword", callbackUrl).ConfigureAwait(false);
            return (IdentityResult.Success, code);
        }

        public async Task<IdentityResult> ResetPassword(string email, string code, string password)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            {
                var userResult = IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" });
                LogErrors("Reset password", userResult);
                return userResult;
            }

            logger.LogInformation("Resetting password for user");
            var result = await userManager.ResetPasswordAsync(user, code, password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                logger.LogInformation("Reset password successfully for user");
            }
            else
            {
                LogErrors("Resetting password", result);
            }

            return result;
        }

        public async Task<UserResult> FinishRegistration(NewUser newUser, string code)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(newUser.Email).ConfigureAwait(false);
            if (user == null)
            {
                return new UserResult(IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" }));
            }

            logger.LogInformation("Finishing user registration");
            var result = await userManager.ResetPasswordAsync(user, code, newUser.Password).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                LogErrors("Finishing registration, resetting password", result);
                return new UserResult(result);
            }

            user.FirstName = newUser.FirstName;
            user.LastName = newUser.LastName;
            result = await userManager.UpdateAsync(user).ConfigureAwait(false);

            if (result.Succeeded)
            {
                newsletterService.SetSubscriptionBackground(newUser.Email, newUser.IsSubscribedNewsletter);
                logger.LogInformation("User created from anonymous crowdaction participant.");
            }
            else
            {
                LogErrors("Finishing registration, updating user", result);
            }

            return new UserResult(user, result);
        }

        public async Task<UserResult> UpdateUser(UpdatedUser updatedUser, ClaimsPrincipal loggedIn)
        {
            IEnumerable<IdentityError> validationResults = ValidationHelper.ValidateAsIdentity(updatedUser, serviceProvider);
            if (validationResults.Any())
            {
                return new UserResult(IdentityResult.Failed(validationResults.ToArray()));
            }

            ApplicationUser? user = await userManager.FindByIdAsync(updatedUser.Id).ConfigureAwait(false);
            if (user == null)
            {
                return new UserResult(IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" }));
            }

            var loggedInUser = await userManager.GetUserAsync(loggedIn).ConfigureAwait(false);

            // need to be logged in as either admin, or the user being updated, only admins can update representsNumberParticipants or change a user to admin
            if (!(loggedIn.IsInRole(AuthorizationConstants.AdminRole) || loggedInUser.Id == user.Id) ||
                (!loggedIn.IsInRole(AuthorizationConstants.AdminRole) && (updatedUser.representsNumberParticipants != user.RepresentsNumberParticipants || updatedUser.IsAdmin)))
            {
                return new UserResult(IdentityResult.Failed(new IdentityError() { Code = "NOPERM", Description = "You don't have permission to update this user" }));
            }

            logger.LogInformation("Updating user");
            user.Email = updatedUser.Email;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.RepresentsNumberParticipants = updatedUser.representsNumberParticipants;
            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);
            if (updatedUser.IsAdmin)
            {
                await userManager.AddToRoleAsync(user, AuthorizationConstants.AdminRole).ConfigureAwait(false);
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, AuthorizationConstants.AdminRole).ConfigureAwait(false);
            }

            if (!result.Succeeded)
            {
                LogErrors("Error updating user", result);
                return new UserResult(result);
            }
            else
            {
                try
                {
                    await newsletterService.SetSubscription(user.Email, updatedUser.IsSubscribedNewsletter).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    var newsletterResult = IdentityResult.Failed(new IdentityError() { Code = "NEWSSUBCR", Description = $"Newsletter subscription failed: {e.Message}" });
                    LogErrors("Error updating user", newsletterResult);
                    return new UserResult(newsletterResult);
                }

                logger.LogInformation("Updated user");
                return new UserResult(user, IdentityResult.Success);
            }
        }

        public async Task<IdentityResult> ChangePassword(ClaimsPrincipal claimsUser, string currentPassword, string newPassword)
        {
            ApplicationUser? user = await userManager.GetUserAsync(claimsUser).ConfigureAwait(false);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" });
            }

            logger.LogInformation("Changing user password");
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
            if (result.Succeeded)
            {
                logger.LogInformation("Successfully changed user password");
            }
            else
            {
                LogErrors("Changing user password", result);
            }

            return result;
        }

        public async Task<int> IngestUserEvent(ClaimsPrincipal trackedUser, JObject eventData, bool canTrack, CancellationToken token)
        {
            logger.LogInformation("Ingesting user event information");
            ApplicationUser user = await userManager.GetUserAsync(trackedUser).ConfigureAwait(false);
            string? trackedUserId = canTrack ? user?.Id : null;
            var userEvent = new UserEvent(eventData.ToString(), DateTime.UtcNow, trackedUserId);
            context.UserEvents.Add(userEvent);
            await context.SaveChangesAsync(token).ConfigureAwait(false);
            return userEvent.Id;
        }

        public async Task<IdentityResult> DeleteUser(string userId, ClaimsPrincipal loggedInUser)
        {
            logger.LogInformation("Deleting user permanently");
            ApplicationUser? user = await userManager.FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" });
            }

            ApplicationUser? loggedIn = await userManager.GetUserAsync(loggedInUser).ConfigureAwait(false);

            if (!(loggedInUser?.IsInRole(AuthorizationConstants.AdminRole) ?? false) && user.Id != loggedIn?.Id)
            {
                var permissionResult = IdentityResult.Failed(new IdentityError() { Code = "NOPERM", Description = "You don't have permission to delete this user" });
                LogErrors("Deleting user", permissionResult);
                return permissionResult;
            }

            List<Crowdaction> crowdactions =
                await context.CrowdactionParticipants
                             .Include(c => c.Crowdaction)
                             .Where(c => c.UserId == userId && c.Crowdaction!.End < DateTime.UtcNow)
                             .Select(c => c.Crowdaction!)
                             .ToListAsync().ConfigureAwait(false);

            foreach (Crowdaction p in crowdactions)
            {
                p.AnonymousUserParticipants += 1;
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            var result = await userManager.DeleteAsync(user).ConfigureAwait(false);

            if (result.Succeeded)
            {
                await emailSender.SendEmailTemplated(user.Email, "CollAction Account Deleted", "DeleteAccount").ConfigureAwait(false);
                logger.LogInformation("Deleted user permanently: {0}", user.Id);
            }
            else
            {
                LogErrors("Deleting user", result);
            }

            return result;
        }

        private void LogErrors(string operation, IdentityResult result)
        {
            foreach (IdentityError e in result.Errors)
            {
                logger.LogError($"{operation}: {e}");
            }
        }
    }
}
