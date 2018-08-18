using Microsoft.Extensions.Localization;
using System;

namespace CollAction.Models
{
    public class FindProjectsViewModel
    {
        private readonly IStringLocalizer _localizer;

        public FindProjectsViewModel(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectProposal { get; set; }

        public string CategoryName { get; set; }

        public string CategoryColorHex { get; set; }

        public string LocationName { get; set; }

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

        public ProjectStatus Status { get; set; }

        public DateTime End { get; set; }

        public DateTime Start { get; set; }

        public string RemainingTime { get; private set; }

        private TimeSpan _remaining;
        public TimeSpan Remaining
        {
            get
            {
                return _remaining;
            }

            set
            {
                _remaining = value;
                if (value.TotalDays < 2)
                    RemainingTime = String.Format("{0} {1}", value.Hours, value.Hours == 1 ? _localizer["hour"] : _localizer["hours"]);
                else
                    RemainingTime = String.Format("{0} {1}", _localizer["days"]);
            }
        }
    }
}