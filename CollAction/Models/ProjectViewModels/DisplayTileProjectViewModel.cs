using Microsoft.Extensions.Localization;
using System;

namespace CollAction.Models
{
    public class DisplayTileProjectViewModel
    {
        private readonly IStringLocalizer _localizer;

        public DisplayTileProjectViewModel(IStringLocalizer localizer, DateTime projectStart, DateTime projectEnd, ProjectStatus projectStatus, bool IsActive, bool isComingSoon, bool isClosed)
        {
            _localizer = localizer;
            setRemainingTime(projectStart, projectEnd);
            setStatusTexts(projectStatus, IsActive, isComingSoon, isClosed);
        }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectProposal { get; set; }

        public string CategoryName { get; set; }

        public string CategoryColorHex { get; set; }

        public string LocationName { get; set; }

        public string RemainingTime { get; set; }

        public string BannerImagePath { get; set; }

        public int Target { get; set; }

        public int Participants { get; set; }

        public int ProgressPercent
        {
            get
            {
                return Math.Min(Participants * 100 / Target, 100);
            }
        }

        public string StatusText { get; set; }

        public string StatusSubText { get; set; }


        private void setRemainingTime(DateTime projectStart, DateTime projectEnd)
        {
            TimeSpan remainingTime = (DateTime.UtcNow >= projectEnd || projectEnd <= projectStart) ? TimeSpan.Zero : projectEnd - (DateTime.UtcNow > projectStart ? DateTime.UtcNow : projectStart);
            
            if (remainingTime.TotalDays < 1)
                RemainingTime = String.Format("{0} {1}", remainingTime.Hours, remainingTime.Hours == 1 ? _localizer["hour"] : _localizer["hours"]);
            else
                RemainingTime = String.Format("{0} {1}", remainingTime.Days, remainingTime.Days == 1 ? _localizer["day"] : _localizer["days"]);
        }

        private void setStatusTexts(ProjectStatus projectStatus, bool IsActive, bool isComingSoon, bool isClosed)
        {
            if (IsActive)
                StatusText = "OPEN!";
            else if (isComingSoon)
                StatusText = "COMING SOON";
            else if (isClosed)
                StatusText = "CLOSED";
            else if (projectStatus == ProjectStatus.Successful)
            {
                StatusText = "CLOSED SUCCESSFUL";
                StatusSubText = "WIN!";
            }
            else if (projectStatus == ProjectStatus.Failed)
            {
                StatusText = "CLOSED FAIL";
                StatusSubText = "OUCH!";
            }
        }

    }
}