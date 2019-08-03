using System;

namespace CollAction.Services.Projects
{
    public class ProjectEmailOptions
    {
        public int MaxNumberProjectEmails { get; set; } = 4;

        public TimeSpan TimeEmailAllowedAfterProjectEnd { get; set; } = TimeSpan.FromDays(180);
    }
}