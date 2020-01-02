using CollAction.Data;
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
    public class UserService : IUserService
    {
        private readonly INewsletterService newsletterService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly ILogger<UserService> logger;
        private readonly ApplicationDbContext context;
        private readonly SiteOptions siteOptions;

        public UserService(
            INewsletterService newsletterService, 
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ILogger<UserService> logger,
            ApplicationDbContext context,
            IOptions<SiteOptions> siteOptions)
        {
            this.newsletterService = newsletterService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.logger = logger;
            this.context = context;
            this.siteOptions = siteOptions.Value;
        }

        public async Task<UserResult> CreateUser(string email, ExternalLoginInfo info)
        {
            logger.LogInformation("Creating user from external login");
            ApplicationUser user = new ApplicationUser()
            {
                Email = email,
                UserName = email,
                RegistrationDate = DateTime.UtcNow,
                RepresentsNumberParticipants = 1
            };
            IdentityResult result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user, info);
                if (!result.Succeeded)
                {
                    LogErrors("Adding external login", result);
                }
                else
                {
                    logger.LogInformation("Created user from external login");
                }

                return new UserResult() { User = user, Result = result };
            }
            else
            {
                LogErrors("Creating user", result);
                return new UserResult() { Result = result };
            }
        }

        public async Task<UserResult> CreateUser(NewUser newUser)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                UserName = newUser.Email,
                RegistrationDate = DateTime.UtcNow,
                RepresentsNumberParticipants = 1
            };
            IdentityResult result = await userManager.CreateAsync(user, newUser.Password);
            if (result.Succeeded)
            {
                newsletterService.SetSubscriptionBackground(newUser.Email, newUser.IsSubscribedNewsletter);
                return new UserResult() { User = user, Result = result };
            }
            else
            {
                LogErrors("Creating user", result);
                return new UserResult() { Result = result };
            }
        }

        public async Task<(IdentityResult Result, string ResetCode)> ForgotPassword(string email)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var result = IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" });
                LogErrors("Forgot password", result);
                return (result, null);
            }

            logger.LogInformation("Sending reset password for user");
            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            string callbackUrl = $"{siteOptions.PublicAddress}/Manage/ResetPassword?code={WebUtility.UrlEncode(code)}&email={WebUtility.UrlEncode(email)}";
            await emailSender.SendEmailTemplated(email, "Reset Password", "ResetPassword", callbackUrl);
            return (IdentityResult.Success, code);
        }

        public async Task<IdentityResult> ResetPassword(string email, string code, string password)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var userResult = IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" });
                LogErrors("Reset password", userResult);
                return userResult;
            }

            logger.LogInformation("Resetting password for user");
            var result = await userManager.ResetPasswordAsync(user, code, password);
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
            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user == null)
            {
                return new UserResult() { Result = IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" }) };
            }

            logger.LogInformation("Finishing user registration");
            var result = await userManager.ResetPasswordAsync(user, code, newUser.Password);
            if (!result.Succeeded)
            {
                LogErrors("Finishing registration, resetting password", result);
                return new UserResult { Result = result };
            }

            user.FirstName = newUser.FirstName;
            user.LastName = newUser.LastName;
            result = await userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                newsletterService.SetSubscriptionBackground(newUser.Email, newUser.IsSubscribedNewsletter);
                logger.LogInformation("User created from anonymous project participant.");
            }
            else
            {
                LogErrors("Finishing registration, updating user", result);
            }

            return new UserResult() { Result = result, User = user };
        }

        public async Task<UserResult> UpdateUser(UpdatedUser updatedUser, ClaimsPrincipal loggedIn)
        {
            var user = await userManager.FindByIdAsync(updatedUser.Id);
            if (user == null)
            {
                return new UserResult() { Result = IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" }) };
            }

            var loggedInUser = await userManager.GetUserAsync(loggedIn);

            // need to be logged in as either admin, or the user being updated, only admins can update RepresentsNumberUsers
            if (!(loggedIn.IsInRole(Constants.AdminRole) || loggedInUser.Id == user.Id) || (!loggedIn.IsInRole(Constants.AdminRole) && updatedUser.RepresentsNumberUsers != user.RepresentsNumberParticipants)) 
            {
                return new UserResult() { Result = IdentityResult.Failed(new IdentityError() { Code = "NOPERM", Description = "You don't have permission to update this user" }) };
            }

            logger.LogInformation("Updating user");
            user.Email = updatedUser.Email;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.RepresentsNumberParticipants = updatedUser.RepresentsNumberUsers;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                LogErrors("Error updating user", result);
                return new UserResult() { Result = result };
            }
            else
            {
                try
                {
                    await newsletterService.SetSubscription(user.Email, updatedUser.IsSubscribedNewsletter);
                }
                catch (Exception e)
                {
                    var newsletterResult = IdentityResult.Failed(new IdentityError() { Code = "NEWSSUBCR", Description = $"Newsletter subscription failed: {e.Message}" });
                    LogErrors("Error updating user", newsletterResult);
                    return new UserResult() { Result = newsletterResult };
                }

                logger.LogInformation("Updated user");
                return new UserResult() { Result = IdentityResult.Success, User = user };
            }
        }

        public async Task<IdentityResult> ChangePassword(ClaimsPrincipal claimsUser, string currentPassword, string newPassword)
        {
            var user = await userManager.GetUserAsync(claimsUser);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "NOUSER", Description = "This user doesn't exist" });
            }

            logger.LogInformation("Changing user password");
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
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
            ApplicationUser user = await userManager.GetUserAsync(trackedUser);
            string trackedUserId = canTrack ? user?.Id : null;
            var userEvent = new UserEvent()
            {
                EventLoggedAt = DateTime.UtcNow,
                EventData = eventData.ToString(),
                UserId = trackedUserId
            };
            context.UserEvents.Add(userEvent);
            await context.SaveChangesAsync(token);
            return userEvent.Id;
        }

        public async Task<IdentityResult> DeleteUser(string userId, ClaimsPrincipal loggedInUser)
        {
            logger.LogInformation("Deleting user permanently");
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            ApplicationUser loggedIn = await userManager.GetUserAsync(loggedInUser);

            if (!loggedInUser.IsInRole(Constants.AdminRole) && user.Id != loggedIn?.Id)
            {
                var permissionResult = IdentityResult.Failed(new IdentityError() { Code = "NOPERM", Description = "You don't have permission to delete this user" });
                LogErrors("Deleting user", permissionResult);
                return permissionResult;
            }

            List<Project> projects =
                await context.ProjectParticipants
                             .Include(p => p.Project)
                             .Where(p => p.UserId == userId && p.Project.End < DateTime.UtcNow)
                             .Select(p => p.Project)
                             .ToListAsync();

            foreach (Project p in projects)
            {
                p.AnonymousUserParticipants += 1;
            }

            await context.SaveChangesAsync();
            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                logger.LogInformation("Deleted user permanently");
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
