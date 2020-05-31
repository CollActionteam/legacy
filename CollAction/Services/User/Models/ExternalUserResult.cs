using CollAction.Models;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Services.User.Models
{
    public sealed class ExternalUserResult
    {
        public ExternalUserResult(ApplicationUser user, IdentityResult result, ExternalLoginInfo info, bool addedUser)
        {
            User = user;
            Result = result;
            AddedUser = addedUser;
            Info = info;
        }

        public ExternalUserResult(IdentityResult result, ExternalLoginInfo info)
        {
            Result = result;
            AddedUser = false;
            Info = info;
        }

        public ApplicationUser? User { get; set; }

        public IdentityResult Result { get; set; }

        public ExternalLoginInfo Info { get; set; }

        public bool AddedUser { get; }
    }
}
