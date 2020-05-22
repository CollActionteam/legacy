using CollAction.Data;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Image;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CollAction.GraphQl.Queries
{
    public sealed class ImageFileGraph : EfObjectGraphType<ApplicationDbContext, ImageFile>
    {
        public ImageFileGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>, int>(nameof(ImageFile.Id)).Resolve(x => x.Source.Id);
            Field<NonNullGraphType<DateTimeOffsetGraphType>, DateTime>(nameof(ImageFile.Date)).Resolve(x => x.Source.Date);
            Field(x => x.Description);
            Field(x => x.Filepath);
            Field(x => x.Height);
            Field(x => x.Width);
            Field<NonNullGraphType<StringGraphType>, string>("url")
                .Resolve(c =>
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
