using System.Threading.Tasks;
using CollAction.Models;

namespace CollAction.Services.Project
{
    public interface IParticipantsService
    {
        Task<bool> AddParticipant(string userId, int projectId);
        Task RefreshParticipantCountMaterializedView();
        Task<ProjectParticipant> GetParticipant(string userId, int projectId);
        Task<string> GenerateParticipantsDataExport(int projectId);
    }
}