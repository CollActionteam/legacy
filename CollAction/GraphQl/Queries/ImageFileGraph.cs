﻿using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    public class ImageFileGraph : EfObjectGraphType<ApplicationDbContext, ImageFile>
    {
        public ImageFileGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Date);
            Field(x => x.Description);
            Field(x => x.Filepath);
            Field(x => x.Height);
            Field(x => x.Width);
        }
    }
}
