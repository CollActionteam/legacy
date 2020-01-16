using CollAction.Models;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Services.User.Models
{
    public sealed class UserResult
    {
        public UserResult(ApplicationUser user, IdentityResult result)
        {
            User = user;
            Result = result;
        }

        public UserResult(IdentityResult result)
        {
            Result = result;
        }

        public ApplicationUser? User { get; set; }

        public IdentityResult Result { get; set; }
    }
}
