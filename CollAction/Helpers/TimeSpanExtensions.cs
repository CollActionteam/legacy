using System;

namespace CollAction.Helpers
{
    /// <summary>
    /// Extension for timespan for years/months, rough approximations, but sufficient
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Get a rough approximation of the amount of years in the timespan (for an exact calculation, you'd need more then a timespan)
        /// </summary>
        /// <param name="timespan">The timespan</param>
        /// <returns>Approximate number of years</returns>
        public static int Years(this TimeSpan timespan)
        {
            return (int)(timespan.Days / 365.2425);
        }

        /// <summary>
        /// Get a rough approximation of the amount of months in the timespan (for an exact calculation, you'd need more then a timespan)
        /// </summary>
        /// <param name="timespan">The timespan</param>
        /// <returns>Approximate number of months</returns>
        public static int Months(this TimeSpan timespan)
        {
            return (int)(timespan.Days / 30.436875);
        }

        /// <summary>
        /// Get number of weeks in the timespan
        /// </summary>
        /// <param name="timespan">The timespan</param>
        /// <returns>Number of weeks</returns>
        public static int Weeks(this TimeSpan timespan)
        {
            return (int)(timespan.Days / 7.0);
        }
    }
}
