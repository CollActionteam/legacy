using System;
using System.Threading.Tasks;
using CollAction.Models;

namespace CollAction.Services.Project
{
    public interface IParticipantsService
    {
        Task AddUnregisteredParticipant(int projectId, string email, Uri projectLink);
        Task AddParticipant(int projectId, string userId, Uri projectLink);
        Task RefreshParticipantCountMaterializedView();
        Task<ProjectParticipant> GetParticipant(string userId, int projectId);
        Task<string> GenerateParticipantsDataExport(int projectId);
    }
}