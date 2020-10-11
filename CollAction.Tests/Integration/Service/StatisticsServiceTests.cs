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
        private readonly IStatisticsService statisticsService;

        public StatisticsServiceTests() : base(false)
        { 
            statisticsService = Scope.ServiceProvider.GetRequiredService<IStatisticsService>();
        }

        [Fact]
        public async Task TestStatistics()
        {
            // Crappy test.. TODO: need to clean the database to do something like this better
            int numberActions = await statisticsService.NumberActionsTaken(CancellationToken.None);
            Assert.True(numberActions >= 0);
            int numberUsers = await statisticsService.NumberUsers(CancellationToken.None);
            Assert.True(numberUsers >= 0);
            int numberCrowdactions = await statisticsService.NumberCrowdactions(CancellationToken.None);
            Assert.True(numberCrowdactions >= 0);
        }
    }
}
