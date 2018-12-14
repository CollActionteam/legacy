using Microsoft.Extensions.Localization;

namespace CollAction.Models.ProjectViewModels
{
    public class ParticipatingInProjectsViewModel : FindProjectsViewModel
    {
        public ParticipatingInProjectsViewModel(IStringLocalizer localizer) : base(localizer)
        {
        }

        public bool SubscribedToEmails { get; set; }
    }
}