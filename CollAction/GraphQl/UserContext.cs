using CollAction.Data;
using System.Security.Claims;

namespace CollAction.GraphQl
{
    public class UserContext
    {
        public ClaimsPrincipal User { get; set; }
        public ApplicationDbContext Context { get; set; }
    }
}
