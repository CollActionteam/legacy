using CollAction.Models;
using CollAction.Services.Crowdactions.Models;
using System;
using System.Net;

namespace CollAction.ViewModels.Email
{
    public sealed class CrowdactionCommitEmailViewModel
    {
        public CrowdactionCommitEmailViewModel(Crowdaction crowdaction, AddParticipantResult result, ApplicationUser? user, Uri publicUrl, Uri crowdactionUrl)
        {
            Crowdaction = crowdaction;
            Result = result;
            User = user;
            PublicUrl = publicUrl;
            CrowdactionUrl = crowdactionUrl;
        }

        public Crowdaction Crowdaction { get; set; }

        public AddParticipantResult Result { get; set; }

        public ApplicationUser? User { get; set; }

        public Uri PublicUrl { get; set; }

        public Uri CrowdactionUrl { get; set; }

        public Uri StartLink
            => new Uri(PublicUrl, "/crowdactions/start");

        public Uri LoginLink
            => new Uri(PublicUrl, "/account/login");

        public Uri FinishRegistrationLink
            => new Uri(PublicUrl, $"/account/finish-registration?email={WebUtility.UrlEncode(Result.ParticipantEmail)}&code={WebUtility.UrlEncode(Result.PasswordResetToken)}");

        public Uri FacebookLink
            => new Uri($"https://www.facebook.com/sharer/sharer.php?u={WebUtility.UrlEncode(CrowdactionUrl.ToString())}");

        public Uri FacebookImageLink
            => new Uri(PublicUrl, "/social/facebook.png");

        public Uri LinkedinLink
            => new Uri($"http://www.linkedin.com/shareArticle?mini=true&url={WebUtility.UrlEncode(CrowdactionUrl.ToString())}&title={WebUtility.UrlEncode(Crowdaction.Name)}");

        public Uri LinkedinImageLink
            => new Uri(PublicUrl, "/social/linkedin.png");

        public Uri TwitterLink
            => new Uri($"https://twitter.com/intent/tweet?text={WebUtility.UrlEncode(Crowdaction.Name)}&url={WebUtility.UrlEncode(CrowdactionUrl.ToString())}");

        public Uri TwitterImageLink
            => new Uri(PublicUrl, "/social/twitter.png");

        public bool ShowAlreadyParticipating
            => Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating ||
               Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating;

        public bool ShowYouCanLogin
            => Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating ||
               Result.Scenario == AddParticipantScenario.AnonymousAlreadyRegisteredAndAdded;

        public bool ShowCreateYourOwnCrowdaction
            => Result.Scenario == AddParticipantScenario.LoggedInAndAdded;

        public bool ShowCreateAccount
            => Result.Scenario == AddParticipantScenario.AnonymousCreatedAndAdded ||
               Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAdded ||
               Result.Scenario == AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating;
    }
}
