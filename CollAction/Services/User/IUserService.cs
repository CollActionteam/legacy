using CollAction.Models;
using CollAction.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.User
{
    public interface IUserService
    {
        Task<ExternalUserResult> CreateOrAddExternalToUser(string email, ExternalLoginInfo info);

        Task<UserResult> CreateUser(NewUser newUser);

        Task<UserResult> UpdateUser(UpdatedUser updatedUser, ClaimsPrincipal claimsUser);

        Task<IdentityResult> DeleteUser(string userId, ClaimsPrincipal loggedInUser);

        Task<(IdentityResult Result, string? ResetCode)> ForgotPassword(string email);

        Task<IdentityResult> ResetPassword(string email, string code, string password);

        Task<IdentityResult> ChangePassword(ClaimsPrincipal claimsUser, string currentPassword, string newPassword);

        Task<UserResult> FinishRegistration(NewUser newUser, string code);

        Task<int> IngestUserEvent(ClaimsPrincipal trackedUser, JObject eventData, bool canTrack, CancellationToken token);

        IQueryable<ApplicationUser> SearchUsers(string? searchString);
    }
}