using CollAction.Models;
using CollAction.Services.Projects.Models;
using System;
using System.Net;

namespace CollAction.ViewModels.Email
{
    public sealed class ProjectCommitEmailViewModel
    {
        public ProjectCommitEmailViewModel(Project project, AddParticipantResult result, ApplicationUser? user, Uri publicAddress, Uri projectUrl)
        {
            Project = project;
            Result = result;
            User = user;
            PublicAddress = publicAddress;
            ProjectUrl = projectUrl;
        }

        public Project Project { get; set; }

        public AddParticipantResult Result { get; set; }

        public ApplicationUser? User { get; set; }

        public Uri PublicAddress { get; set; }

        public Uri ProjectUrl { get; set; }

        public Uri StartLink
            => new Uri($"{PublicAddress}/projects/start");

        public Uri FinishRegistrationLink
            => new Uri($"{PublicAddress}/account/finish-registration?email={WebUtility.UrlEncode(Result.ParticipantEmail)}&code={WebUtility.UrlEncode(Result.PasswordResetToken)}");

        public string FacebookLink
            => $"https://www.facebook.com/sharer/sharer.php?u={ProjectUrl}";

        public string LinkedinLink
            => $"http://www.linkedin.com/shareArticle?mini=true&url={ProjectUrl}&title={WebUtility.UrlEncode(Project.Name)}";

        public string TwitterLink
            => $"https://twitter.com/intent/tweet?text={WebUtility.UrlEncode(Project.Name)}&url={ProjectUrl}";

        public bool ShowAlreadyParticipating
            => Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating ||
               Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating;

        public bool ShowYouCanLogin
            => Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating ||
               Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAdded;

        public bool ShowCreateYourOwnProject
            => Result.Scenario == AddParticipantScenario.LoggedInAndAdded;

        public bool ShowCreateAccount
            => Result.Scenario == AddParticipantScenario.AnonymousCreatedAndAdded ||
               Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAdded ||
               Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating;
    }
}
