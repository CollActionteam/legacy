using CollAction.Models;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Services.User.Models
{
    public sealed class ExternalUserResult
    {
        public ExternalUserResult(ApplicationUser user, IdentityResult result, bool addedUser)
        {
            User = user;
            Result = result;
            AddedUser = addedUser;
        }

        public ExternalUserResult(IdentityResult result)
        {
            Result = result;
            AddedUser = false;
        }

        public ApplicationUser? User { get; set; }

        public IdentityResult Result { get; set; }

        public bool AddedUser { get; }
    }
}
