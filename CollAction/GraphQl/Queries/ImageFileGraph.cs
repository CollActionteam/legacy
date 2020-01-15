using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Image;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Queries
{
    public class ImageFileGraph : EfObjectGraphType<ApplicationDbContext, ImageFile>
    {
        public ImageFileGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService, IServiceScopeFactory serviceScopeFactory) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Date);
            Field(x => x.Description);
            Field(x => x.Filepath);
            Field(x => x.Height);
            Field(x => x.Width);
            Field<StringGraphType>(
                "url",
                resolve: c =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    return scope.ServiceProvider
                                .GetRequiredService<IImageService>()
                                .GetUrl(c.Source);
                });
        }
    }
}
