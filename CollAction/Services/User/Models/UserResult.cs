using CollAction.Models;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Services.User.Models
{
    public sealed class UserResult
    {
        public ApplicationUser User { get; set; }

        public IdentityResult Result { get; set; }
    }
}
