using CollAction.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public interface IProjectService
    {
        Task<Project> GetProjectById(int? id);

        Task<IEnumerable<Project>> GetProjects(Expression<Func<Project, bool>> WhereExpression);

        Task<bool> AddParticipant(string userId, int projectId);

        Task<ProjectParticipant> GetParticipant(string userId, int projectId);

        Task<IEnumerable<DisplayTileProjectViewModel>> GetTileProjects(Expression<Func<Project, bool>> WhereExpression);
    }
}
