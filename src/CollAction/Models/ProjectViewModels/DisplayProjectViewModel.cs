using CollAction.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CollAction.Models
{
    public class DisplayProjectViewModel
    {
        public Project Project { get; set; }

        public int Participants { get; set; }

        public bool HasDescriptionVideo { get { return Project.DescriptionVideoLink != null; } }

        public string DescriptionVideoYouTubeEmbedLink {
            get {
                return HasDescriptionVideo ? "https://www.youtube.com/embed/" + YouTubeId : "";
            }
        }

        private string YouTubeId {
            get {
                // Extract the YouTubeId from a link of this form http://www.youtube.com/watch?v=-wtIMTCHWuI
                Uri uri = new Uri(Project.DescriptionVideoLink.Link);
                var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
                Microsoft.Extensions.Primitives.StringValues youTubeId;
                return queryDictionary.Count == 1 && queryDictionary.TryGetValue("v", out youTubeId) ? youTubeId.ToString() : "";
            }
        }

        public int RemainingDays
            => Convert.ToInt32(Math.Round((Project.End - Project.Start).TotalDays));

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                if (Project.Status == ProjectStatus.Hidden) { return "hidden"; }
                else if (Project.IsActive) { return String.Format("open, {0} days left", RemainingDays); }
                else if (Project.IsComingSoon) { return "coming soon"; }
                else if (Project.IsClosed) { return "closed"; }
                else if (Project.Status == ProjectStatus.Successful) { return "successful"; }
                else if (Project.Status == ProjectStatus.Failed) { return "failed"; }
                else if (Project.Status == ProjectStatus.Deleted) { return "deleted"; }
                else { return "undefined"; }
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