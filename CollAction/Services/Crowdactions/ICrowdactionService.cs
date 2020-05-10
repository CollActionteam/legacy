using CollAction.GraphQl.Mutations.Input;
using CollAction.Models;
using CollAction.Services.Crowdactions.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Crowdactions
{
    public interface ICrowdactionService
    {
        Task<CrowdactionResult> CreateCrowdaction(NewCrowdaction newCrowdaction, ClaimsPrincipal user, CancellationToken token);

        Task<Crowdaction> CreateCrowdactionInternal(NewCrowdactionInternal newCrowdaction, CancellationToken token);

        Task<CrowdactionResult> UpdateCrowdaction(UpdatedCrowdaction updatedCrowdaction, ClaimsPrincipal user, CancellationToken token);

        Task<int> DeleteCrowdaction(int id, CancellationToken token);

        Task<AddParticipantResult> CommitToCrowdactionAnonymous(string email, int crowdactionId, CancellationToken token);

        Task<AddParticipantResult> CommitToCrowdactionLoggedIn(ClaimsPrincipal user, int crowdactionId, CancellationToken token);

        Task<CrowdactionResult> SendCrowdactionEmail(int crowdactionId, string subject, string message, ClaimsPrincipal performingUser, CancellationToken token);

        Task<CrowdactionParticipant> SetEmailCrowdactionSubscription(int crowdactionId, string userId, Guid token, bool isSubscribed, CancellationToken cancellationToken);

        bool CanSendCrowdactionEmail(Crowdaction crowdaction);

        IQueryable<Crowdaction> SearchCrowdactions(Category? category, SearchCrowdactionStatus? status);

        Task RefreshParticipantCount(CancellationToken token);

        void InitializeRefreshParticipantCountJob();
    }
}