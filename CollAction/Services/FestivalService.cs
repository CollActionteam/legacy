using System;
using Microsoft.Extensions.Options;

namespace CollAction.Services
{
    public class FestivalService : IFestivalService
    {
        private readonly FestivalServiceOptions _festivalOptions;
        private readonly DateTime DefaultEndDate = new DateTime(2018, 5, 27, 23, 59, 59);

        public FestivalService(IOptions<FestivalServiceOptions> festivalOptions)
        {
            _festivalOptions = festivalOptions.Value;
        }

        public bool CtasVisible => DateTime.Now < (_festivalOptions.FestivalEndDate ?? DefaultEndDate);
    }
}