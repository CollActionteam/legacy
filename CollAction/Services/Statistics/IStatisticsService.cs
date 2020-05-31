using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Statistics
{
    public interface IStatisticsService
    {
        Task<int> NumberUsers(CancellationToken token);

        Task<int> NumberCrowdactions(CancellationToken token);

        Task<int> NumberActionsTaken(CancellationToken token);
    }
}
