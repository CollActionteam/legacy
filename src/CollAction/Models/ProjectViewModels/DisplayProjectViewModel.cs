using CollAction.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollAction.Models
{
    public class DisplayProjectViewModel
    {
        public Project Project { get; set; }

        public int Participants { get; set; }

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