using CollAction.Services.Statistics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration.Service
{
    [TestClass]
    public class StatisticsServiceTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestStatistics()
            => WithServiceProvider(
                   async scope =>
                   {
                       // Crappy test.. TODO: need to clean the database to do something like this better
                       IStatisticsService statisticsService = scope.ServiceProvider.GetRequiredService<IStatisticsService>();
                       int numberActions = await statisticsService.NumberActionsTaken(CancellationToken.None).ConfigureAwait(false);
                       Assert.IsTrue(numberActions >= 0);
                       int numberUsers = await statisticsService.NumberUsers(CancellationToken.None).ConfigureAwait(false);
                       Assert.IsTrue(numberUsers >= 0);
                       int numberProjects = await statisticsService.NumberProjects(CancellationToken.None).ConfigureAwait(false);
                       Assert.IsTrue(numberProjects >= 0);
                   });
    }
}
