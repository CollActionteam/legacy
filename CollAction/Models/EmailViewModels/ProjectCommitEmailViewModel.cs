using CollAction.Services.Project;
using System.Net;

namespace CollAction.Models.EmailViewModels
{
    public class ProjectCommitEmailViewModel
    {
        public Project Project { get; set; }
        public AddParticipantResult Result { get; set; }
        public ApplicationUser LoggedInUser { get; set; }
        public string PublicAddress { get; set; }
        public string ProjectUrl { get; set; }

        public string StartLink
        {
            get
            {
                return $"{PublicAddress}/Start";
            }
        }

        public string FinishRegistrationLink
        {
            get
            {
                return $"{PublicAddress}/account/FinishRegistration?email={WebUtility.UrlEncode(Result.ParticipantEmail)}&code={WebUtility.UrlEncode(Result.PasswordResetToken)}";
            }
        }

        public string FacebookLink
        {
            get
            {
                return $"https://www.facebook.com/sharer/sharer.php?u={ProjectUrl}";
            }
        }

        public string LinkedinLink
        {
            get
            {
                return $"http://www.linkedin.com/shareArticle?mini=true&url={ProjectUrl}&title={WebUtility.UrlEncode(Project.Name)}";
            }
        }

        public string TwitterLink
        {
            get
            {
                return $"https://twitter.com/intent/tweet?text={WebUtility.UrlEncode(Project.Name)}&url={ProjectUrl}";
            }
        }

        public bool ShowAlreadyParticipating
        {
            get
            {
                return Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating ||
                       Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating;
            }
        }

        public bool ShowYouCanLogin
        {
            get
            {
                return Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating ||
                       Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAdded;
            }
        }

        public bool ShowCreateYourOwnProject
        {
            get
            {
                return Result.Scenario == AddParticipantScenario.LoggedInAndAdded;
            }
        }

        public bool ShowCreateAccount
        {
            get
            {
                return Result.Scenario == AddParticipantScenario.AnonymousCreatedAndAdded ||
                       Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAdded ||
                       Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating;
            }
        }
    }
}
