using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Image;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using CollAction.Helpers;

namespace CollAction.GraphQl.Queries
{
    public sealed class ImageFileGraph : EfObjectGraphType<ApplicationDbContext, ImageFile>
    {
        public ImageFileGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>>(nameof(ImageFile.Id), resolve: x => x.Source.Id);
            Field<NonNullGraphType<DateTimeOffsetGraphType>>(nameof(ImageFile.Date), resolve: x => x.Source.Date);
            Field(x => x.Description);
            Field(x => x.Filepath);
            Field(x => x.Height);
            Field(x => x.Width);
            Field<NonNullGraphType<StringGraphType>>(
                "url",
                resolve: c =>
                {
                    return c.GetUserContext()
                            .ServiceProvider
                            .GetRequiredService<IImageService>()
                            .GetUrl(c.Source)
                            .ToString();
                });
        }
    }
}
