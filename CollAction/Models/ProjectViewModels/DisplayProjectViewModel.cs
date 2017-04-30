using CollAction.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollAction.Helpers;

namespace CollAction.Models
{
    public class DisplayProjectViewModel
    {
        public Project Project { get; set; }

        public int Participants { get; set; }

        public bool IsUserCommitted { get; set; } = false;

        public bool HasDescriptionVideo { get { return Project.DescriptionVideoLink != null; } }

        public string DescriptionVideoYouTubeEmbedLink
            => HasDescriptionVideo ? "https://www.youtube.com/embed/" + YouTubeId : "";

        private string YouTubeId
        {
            get
            {
                // Extract the YouTubeId from a link of this form http://www.youtube.com/watch?v=-wtIMTCHWuI
                Uri uri = new Uri(Project.DescriptionVideoLink.Link);
                var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
                Microsoft.Extensions.Primitives.StringValues youTubeId;
                return queryDictionary.Count == 1 && queryDictionary.TryGetValue("v", out youTubeId) ? youTubeId.ToString() : "";
            }
        }

        public TimeSpan RemainingTime
            => Project.End - DateTime.UtcNow;

        public string RemainingTimeUserFriendly
        {
            get
            {
                if (RemainingTime.Years() > 1)
                    return $"{RemainingTime.Years()} years";
                else if (RemainingTime.Months() > 1)
                    return $"{RemainingTime.Months()} months";
                else if (RemainingTime.Weeks() > 1)
                    return $"{RemainingTime.Weeks()} weeks";
                else if (RemainingTime.Days > 1)
                    return $"{RemainingTime.Days} days";
                else if (RemainingTime.Hours > 1)
                    return $"{RemainingTime.Hours} hours";
                else
                    return $"{RemainingTime.Minutes} minutes";
            }
        }

        [Display(Name = "Progress")]
        public int Progress
        {
            get
            {
                return Participants * 100 / Project.Target; // N.B Project.Target is by definition >= 1 so no chance of divide by zero.
            }                    
        }

        public static async Task<List<DisplayProjectViewModel>> GetViewModelsWhere(ApplicationDbContext context, Expression<Func<Project, bool>> WhereExpression)
        {
            return await context.Projects
                .Where(WhereExpression)
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.BannerImage)
                .Include(p => p.DescriptionVideoLink)
                .Include(p => p.Owner)
                .GroupJoin(context.ProjectParticipants,
                    project => project.Id,
                    participants => participants.ProjectId,
                    (project, participantsGroup) => new DisplayProjectViewModel
                    {
                        Project = project,
                        Participants = participantsGroup.Count()
                    })
                .ToListAsync();
        }
    }
}