using System;
using Microsoft.Extensions.Options;

namespace CollAction.Services.Festival
{
    public class FestivalService : IFestivalService
    {
        private readonly FestivalServiceOptions festivalOptions;

        public FestivalService(IOptions<FestivalServiceOptions> festivalOptions)
        {
            this.festivalOptions = festivalOptions.Value;
        }

        public bool FestivalCallToActionVisible 
            => festivalOptions.FestivalEndDate.HasValue && festivalOptions.FestivalEndDate >= DateTime.Now;
    }
}