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
            FieldAsync<NonNullGraphType<IntGraphType>, int>(
                "numberActionsTaken",
                resolve: c =>
                {
                    return c.GetUserContext()
                            .ServiceProvider
                            .GetRequiredService<IStatisticsService>()
                            .NumberActionsTaken(c.CancellationToken)
                });

            FieldAsync<NonNullGraphType<IntGraphType>, int>(
                "numberCrowdactions",
                resolve: c =>
                {
                    return c.GetUserContext()
                            .ServiceProvider
                            .GetRequiredService<IStatisticsService>()
                            .NumberCrowdactions(c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<IntGraphType>, int>(
                "numberUsers",
                resolve: c =>
                {
                    return c.GetUserContext()
                            .ServiceProvider
                            .GetRequiredService<IStatisticsService>()
                            .NumberUsers(c.CancellationToken);
                });
        }
    }
}
