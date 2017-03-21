using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace CollAction.Models
{
    public class DisplayTileProjectViewModel
    {
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

        public int ProgressPercent { get; set; }

        public string StatusText { get; set; }

        public string StatusSubText { get; set; }


        public void setRemainingTime(TimeSpan remainingTime)
        {
            if (remainingTime.TotalDays < 1)
                RemainingTime = String.Format("{0} {1}", remainingTime.Hours, remainingTime.Hours == 1 ? "hour" : "hours");
            else
                RemainingTime = String.Format("{0} {1}", remainingTime.Days, remainingTime.Days == 1 ? "day" : "days");
        }

        public void setStatusTexts(ProjectStatus projectStatus, bool IsActive, bool isComingSoon, bool isClosed)
        {
            if (IsActive)
                StatusText = "OPEN!";
            else if (isComingSoon)
                StatusText = "COMING SOON";
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