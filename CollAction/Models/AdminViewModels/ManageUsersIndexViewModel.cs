using System.Collections.Generic;

namespace CollAction.Models.AdminViewModels
{
    public class ManageUsersIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage
            => CurrentPage + 1;
        public int PreviousPage
            => CurrentPage - 1;
        public int NumberPages { get; internal set; }
    }
}
