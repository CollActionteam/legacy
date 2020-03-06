using CollAction.Helpers;
using CollAction.Services.Statistics;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Queries
{
    public class StatisticsGraph : ObjectGraphType
    {
        public StatisticsGraph()
        {
            FieldAsync<IntGraphType>(
                "numberActionsTaken",
                resolve: async c =>
                {
                    return await c.GetUserContext()
                                  .ServiceProvider
                                  .GetRequiredService<IStatisticsService>()
                                  .NumberActionsTaken(c.CancellationToken)
                                  .ConfigureAwait(false);
                });

            FieldAsync<IntGraphType>(
                "numberProjects",
                resolve: async c =>
                {
                    return await c.GetUserContext()
                                  .ServiceProvider
                                  .GetRequiredService<IStatisticsService>()
                                  .NumberProjects(c.CancellationToken)
                                  .ConfigureAwait(false);
                });

            FieldAsync<IntGraphType>(
                "numberUsers",
                resolve: async c =>
                {
                    return await c.GetUserContext()
                                  .ServiceProvider
                                  .GetRequiredService<IStatisticsService>()
                                  .NumberUsers(c.CancellationToken)
                                  .ConfigureAwait(false);
                });
        }
    }
}
