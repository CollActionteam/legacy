using System.Security.Claims;
using System.Threading.Tasks;
using CollAction.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;

namespace CollAction.Services.User
{
    public interface IUserService
    {
        Task<UserResult> CreateUser(NewUser newUser);

        Task<UserResult> UpdateUser(UpdatedUser updatedUser, ClaimsPrincipal loggedInUser);

        Task<IdentityResult> DeleteUser(ClaimsPrincipal user);

        Task<IdentityResult> ForgotPassword(string email);

        Task<IdentityResult> ResetPassword(string email, string code, string password);

        Task<IdentityResult> ChangePassword(ClaimsPrincipal user, string currentPassword, string newPassword);

        Task<IdentityResult> FinishRegistration(NewUser newUser, string code);

        Task<int> IngestUserEvent(ClaimsPrincipal trackedUser, JObject eventData, bool canTrack);
    }
}