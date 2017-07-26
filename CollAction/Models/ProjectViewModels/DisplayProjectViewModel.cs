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
            => HasDescriptionVideo ? $"https://www.youtube.com/embed/{YouTubeId}" : "";

        public string BannerImagePath
            => Project.BannerImage?.Filepath ?? $"/images/default_banners/{Project.Category.Name}.jpg";

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
        
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime Start 
        {
            get
            {
                return Project.End;
            }
        }

        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime End 
        {
            get
            {
                return Project.End;
            }
        }

        public TimeSpan RemainingTime
        {
            get
            {
                TimeSpan remaining = Project.End - DateTime.UtcNow;
                if (remaining.Ticks < 0)
                    return new TimeSpan(0);
                else
                    return remaining;

            }
        }

        public string RemainingTimeUserFriendly
        {
            get
            {
                TimeSpan remaining = RemainingTime;
                if (remaining.Years() > 1)
                    return $"{remaining.Years()} years";
                else if (remaining.Months() > 1)
                    return $"{remaining.Months()} months";
                else if (remaining.Weeks() > 1)
                    return $"{remaining.Weeks()} weeks";
                else if (remaining.Days > 1)
                    return $"{remaining.Days} days";
                else if (remaining.Hours > 1)
                    return $"{remaining.Hours} hours";
                else if (remaining.Minutes > 0)
                    return $"{remaining.Minutes} minutes";
                else
                    return "Done";
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
                .Include(p => p.DescriptiveImage)
                .Include(p => p.DescriptionVideoLink)
                .Include(p => p.Owner)
                .Include(p => p.Tags).ThenInclude(t => t.Tag)
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