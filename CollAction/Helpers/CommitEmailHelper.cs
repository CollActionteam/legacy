using System;
using System.Net;
using CollAction.Models;
using CollAction.Services.Project;

namespace CollAction.Helpers
{
    public class CommitEmailHelper 
    {
        public readonly Project Project;
        private readonly AddParticipantScenario _scenario;
        private readonly ApplicationUser _loggedInUser;
        private readonly string _systemUrl;
        private readonly string _projectUrl;

        public CommitEmailHelper(Project project, AddParticipantScenario scenario, ApplicationUser loggedInUser, string systemUrl, string projectUrl)
        {
            Project = project;

            _scenario = scenario;
            _loggedInUser = loggedInUser;
            _systemUrl = systemUrl;
            _projectUrl = projectUrl;
        }
        
        public string GenerateSubject() =>
            $"Thank you for participating in the \"{Project.Name}\" project on CollAction";

        public string GenerateEmail() 
        {
            switch (_scenario)
            {
                case AddParticipantScenario.LoggedInAndAdded:
                    return 
                        GenerateSalutation() +
                        GenerateThanksForParticipating() +
                        GenerateRegards() +
                        GenerateCreateYourOwnProject();
                case AddParticipantScenario.AnonymousCreatedAndAdded:
                case AddParticipantScenario.AnonymousNotRegisteredPresentAndAdded:
                    return
                        GenerateSalutation() +
                        GenerateThanksForParticipating() +
                        GenerateCreateAccount() +
                        GenerateRegards();
                case AddParticipantScenario.AnonymousAlreadyRegisteredAndAdded:
                    return
                        GenerateSalutation() +
                        GenerateThanksForParticipating() +
                        GenerateYouCanLogIn() +
                        GenerateRegards();
                case AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating:
                    return
                        GenerateSalutation() +
                        GenerateAlreadyParticipating() +
                        GenerateYouCanLogIn() +
                        GenerateRegards();
                case AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating:
                    return
                        GenerateSalutation() +
                        GenerateAlreadyParticipating() +
                        GenerateCreateAccount() +
                        GenerateRegards();
                default:
                    throw new InvalidOperationException($"There is no e-mail for scenario {_scenario}");
            }
        }

        private string GenerateSalutation() =>
            (_loggedInUser?.FirstName != null ? $"Hi {_loggedInUser.FirstName}!" : "Hi!") + "<br/><br/>";

        private string GenerateThanksForParticipating()
        {
            return 
                "Thank you for participating in a CollAction project!<br><br>" +

                "In crowdacting, we only act collectively when we meet the target before the deadline, " +
                "so please feel very welcome to share this project on social media through the social media buttons " +
                $"below and on the <a href=\"{_projectUrl}\">{Project.Name} project page</a>!<br><br>" +

                "We'll keep you updated on the project. Also feel free to Like us on <a href=\"https://www.facebook.com/collaction.org/\">Facebook</a> " +
                "to stay up to date on everything CollAction!<br><br>";
        }

        private string GenerateAlreadyParticipating()
        {
            return 
                $"Thanks for your interest in the {Project.Name} CollAction project, but it appears you are already participating.<br><br>";
        }

        private string GenerateYouCanLogIn()
        {
            return 
                "You can <a href=\"https://collaction.org/account/login\">log in</a> to keep track of the projects you are committed to, " +
                "and even <a href=\"https://collaction.org/start\">start your own projects!</a><br/><br/>";
        }

        private string GenerateCreateAccount() 
        {
            return 
                "Did you know you can easily create an account with us? You'll be able to keep track of the projects you are committed to, " +
                "and even <a href=\"https://collaction.org/start\">start your own projects!</a><br/><br/>";
        }        

        private string GenerateRegards()
        {
            return 
                "Warm regards,<br>The CollAction team<br><br>" +

                "<span style='#share-buttons img {}'>"+
                    "<div id='share-buttons'>"+
                        "<p>Multiply your impact and share the project with the buttons below 🙂</p>"+
                        $"<a href=https://www.facebook.com/sharer/sharer.php?u={ _projectUrl }>"+
                            $"<img style='width: 25px; padding: 5px;border: 0;box-shadow: 0;display: inline;' src=\"{ _systemUrl }\"/images/social/facebook.png alt='Facebook' />" +
                        "</a>" +
                        $"<a href=\"http://www.linkedin.com/shareArticle?mini=true&url={ _projectUrl }&title={ WebUtility.UrlEncode(Project.Name) }\" target=\"_blank\">" +
                            $"<img style='width: 25px; padding: 5px;border: 0;box-shadow: 0;display: inline;' src=\"{ _systemUrl }\"/images/social/linkedin.png alt='LinkedIn' />" +
                        "</a>" +
                        $"<a href=\"https://twitter.com/intent/tweet?text={ WebUtility.UrlEncode(Project.Name) }&url={_projectUrl}\" target=\"_blank\">" +
                            $"<img style='width: 25px; padding: 5px;border: 0;box-shadow: 0;display: inline;' src=\"{ _systemUrl }/images/social/twitter.png\" alt='Twitter' />" +
                        "</a>" +
                    "</div>" +
                "</span>" +
                "<br/><br/>";
        }

        private string GenerateCreateYourOwnProject()
        {
            return "PS: Did you know you can start your own project on <a href=\"https://collaction.org/start\">www.collaction.org/start</a>?<br><br>";
        }
    }
}