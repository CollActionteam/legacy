using Microsoft.Extensions.Localization;
using System;

namespace CollAction.Models
{
    public class DisplayTileProjectViewModel
    {
        private readonly IStringLocalizer _localizer;

        public DisplayTileProjectViewModel(IStringLocalizer localizer, DateTime projectStart, DateTime projectEnd, ProjectStatus projectStatus, bool isActive, bool isComingSoon, bool isClosed)
        {
            _localizer = localizer;
            SetRemainingTime(projectStart, projectEnd);
            SetRemainingTimeText(projectStart, projectEnd);
            SetStatusTexts(projectStatus, isActive, isComingSoon, isClosed);
        }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectProposal { get; set; }

        public string CategoryName { get; set; }

        public string CategoryColorHex { get; set; }

        public string LocationName { get; set; }

        public int RemainingTime { get; set; }

        public string RemainingTimeText { get; set; }

        public string BannerImagePath { get; set; }

        public string BannerImageDescription { get; set; }

        public string DescriptiveImagePath { get; set; }

        public string DescriptiveImageDescription { get; set; }

        public int Target { get; set; }

        public int Participants { get; set; }

        public int ProgressPercent
        {
            get
            {
                return Participants * 100 / Target;
            }
        }

        public string Status { get; set; }

        public string StatusText { get; set; }

        public string StatusSubText { get; set; }

        private void SetRemainingTime(DateTime projectStart, DateTime projectEnd)
        {
            TimeSpan remainingTime = (DateTime.UtcNow >= projectEnd || projectEnd <= projectStart) ? TimeSpan.Zero : projectEnd - (DateTime.UtcNow > projectStart ? DateTime.UtcNow : projectStart);
            RemainingTime = remainingTime.TotalDays < 1 ? remainingTime.Hours : remainingTime.Days;
        }

        private void SetRemainingTimeText(DateTime projectStart, DateTime projectEnd)
        {
            TimeSpan remainingTime = (DateTime.UtcNow >= projectEnd || projectEnd <= projectStart) ? TimeSpan.Zero : projectEnd - (DateTime.UtcNow > projectStart ? DateTime.UtcNow : projectStart);
            if (Math.Floor(remainingTime.TotalDays) < 1)
            {
                RemainingTimeText = remainingTime.TotalHours < 1 && remainingTime.TotalHours != 0 ? "Uur te gaan" : "Uren te gaan";
            }
            else
            {
                RemainingTimeText = Math.Floor(remainingTime.TotalDays) == 1 ? "Dag te gaan" : "Dagen te gaan";
            }
        }

        private void SetStatusTexts(ProjectStatus projectStatus, bool IsActive, bool isComingSoon, bool isClosed)
        {
            if (IsActive)
            {
                Status = "open";
                StatusText = _localizer["OPEN!"];
            }
            else if (isComingSoon)
            {
                Status = "comingSoon";
                StatusText = _localizer["COMING SOON"];
            }
            else if (isClosed)
            {
                Status = "closed";
                StatusText = _localizer["CLOSED"];
            }
            else if (projectStatus == ProjectStatus.Successful)
            {
                Status = "successful";
                StatusText = _localizer["CLOSED SUCCESSFUL"];
                StatusSubText = _localizer["WIN!"];
            }
            else if (projectStatus == ProjectStatus.Failed)
            {
                Status = "fail";
                StatusText = _localizer["CLOSED FAIL"];
                StatusSubText = _localizer["OUCH!"];
            }
        }

    }
}