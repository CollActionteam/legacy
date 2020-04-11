using CollAction.Helpers;
using CollAction.Services.Statistics;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Queries
{
    public sealed class StatisticsGraph : ObjectGraphType
    {
        public StatisticsGraph()
        {
            FieldAsync<NonNullGraphType<IntGraphType>>(
                "numberActionsTaken",
                resolve: async c =>
                {
                    return await c.GetUserContext()
                                  .ServiceProvider
                                  .GetRequiredService<IStatisticsService>()
                                  .NumberActionsTaken(c.CancellationToken)
                                  .ConfigureAwait(false);
                });

            FieldAsync<NonNullGraphType<IntGraphType>>(
                "numberProjects",
                resolve: async c =>
                {
                    return await c.GetUserContext()
                                  .ServiceProvider
                                  .GetRequiredService<IStatisticsService>()
                                  .NumberProjects(c.CancellationToken)
                                  .ConfigureAwait(false);
                });

            FieldAsync<NonNullGraphType<IntGraphType>>(
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
