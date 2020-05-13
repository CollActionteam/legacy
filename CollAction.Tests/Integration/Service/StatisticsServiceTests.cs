using CollAction.Services.Statistics;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration.Service
{
    [Trait("Category", "Integration")]
    public sealed class StatisticsServiceTests : IntegrationTestBase
    {
        public StatisticsServiceTests() : base(false)
        { }

        [Fact]
        public async Task TestStatistics()
        {
            // Crappy test.. TODO: need to clean the database to do something like this better
            IStatisticsService statisticsService = Scope.ServiceProvider.GetRequiredService<IStatisticsService>();
            int numberActions = await statisticsService.NumberActionsTaken(CancellationToken.None).ConfigureAwait(false);
            Assert.True(numberActions >= 0);
            int numberUsers = await statisticsService.NumberUsers(CancellationToken.None).ConfigureAwait(false);
            Assert.True(numberUsers >= 0);
            int numberCrowdactions = await statisticsService.NumberCrowdactions(CancellationToken.None).ConfigureAwait(false);
            Assert.True(numberCrowdactions >= 0);
        }
    }
}
