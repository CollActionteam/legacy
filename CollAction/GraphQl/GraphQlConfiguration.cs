using CollAction.Data;
using CollAction.Helpers;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;
using GraphQL.Utilities;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace CollAction.GraphQl
{
    public static class GraphQlConfiguration
    {
        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddSingleton<IDependencyResolver>(provider => new FuncDependencyResolver(provider.GetRequiredService));
            services.AddSingleton<ISchema, GraphQlSchema>();

            // Ensure queries are executed in serial instead of parallel
            services.AddSingleton<IDocumentExecuter, EfDocumentExecuter>();

            // Register the context
            EfGraphQLConventions.RegisterInContainer<ApplicationDbContext>(
                services,
                model: GetDbModel());

            // Ensure we can use graphql connections
            EfGraphQLConventions.RegisterConnectionTypesInContainer(services);

            Type[] baseClasses = new[]
            {
                typeof(ComplexGraphType<>),
                typeof(EnumerationGraphType<>)
            };

            foreach (var type in GetGraphQlTypes())
            {
                services.AddSingleton(type);

                foreach (var baseClass in baseClasses)
                {
                    if (type.IsAssignableToGenericType(baseClass))
                    {
                        Type? generic = type.GetGenericBaseClass(baseClass);
                        Debug.Assert(generic != null, $"{type.Name} must have geneneric baseclass");
                        Type source = generic.GenericTypeArguments[0];
                        GraphTypeTypeRegistry.Register(source, type);
                        break;
                    }
                }
            }
        }

        public static void AddGraphQlAuth(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<AuthorizationValidationRule>();
            services.AddSingleton(s =>
            {
                var authSettings = new AuthorizationSettings();

                authSettings.AddPolicy(AuthorizationConstants.GraphQlAdminPolicy, _ => _.RequireClaim(ClaimTypes.Role, AuthorizationConstants.AdminRole));

                return authSettings;
            });
            services.AddSingleton<IEnumerable<IValidationRule>>(s =>
            {
                List<IValidationRule> validationRules = DocumentValidator.CoreRules();
                AuthorizationValidationRule authorizationRule = s.GetRequiredService<AuthorizationValidationRule>();
                validationRules.Add(authorizationRule);
                return validationRules;
            });
        }

        private static IModel GetDbModel()
        {
            // Register the context
            using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql("_").Options);
            return context.Model;
        }

        private static IEnumerable<Type> GetGraphQlTypes()
        {
            return typeof(Startup)
                .Assembly
                .GetTypes()
                .Where(x => !x.IsAbstract &&
                            (typeof(IObjectGraphType).IsAssignableFrom(x) ||
                             typeof(IInputObjectGraphType).IsAssignableFrom(x) ||
                             typeof(EnumerationGraphType).IsAssignableFrom(x)));
        }
    }
}
