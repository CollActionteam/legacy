using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Donation;
using CollAction.Services.Newsletter;
using GraphQL;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Queries
{
    public class ApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public ApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService, IDependencyResolver dependencyResolver, IServiceScopeFactory serviceScopeFactory): base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Email);
            Field(x => x.EmailConfirmed);
            Field(x => x.FirstName);
            Field(x => x.FullName);
            Field(x => x.LastName);
            Field(x => x.RepresentsNumberParticipants);
            Field(x => x.UserName);
            FieldAsync<BooleanGraphType>(
                "IsSubscribed", 
                resolve: async c =>
                {
                    return await dependencyResolver.Resolve<INewsletterService>().IsSubscribedAsync(c.Source.Email);
                });
            FieldAsync<ListGraphType<SubscriptionGraphType>>(
                "DonationSubscriptions",
                resolve: async c =>
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IDonationService>().GetSubscriptionsFor(c.Source);
                    }
                });
            AddNavigationListField(nameof(ApplicationUser.Projects), c => c.Source.Projects);
            AddNavigationListField(nameof(ApplicationUser.Participates), c => c.Source.Participates);
        }
    }
}
