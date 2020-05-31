using CollAction.Data;
using CollAction.GraphQl;
using CollAction.ViewModels.GraphQl;
using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("graphql")]
    [ApiController]
    public sealed class GraphQlController : Controller
    {
        private readonly IDocumentExecuter executer;
        private readonly IEnumerable<IValidationRule> validationRules;
        private readonly ApplicationDbContext context;
        private readonly ILogger<GraphQlController> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IMemoryCache cache;
        private readonly ISchema schema;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(1);

        private class CacheKey : IEquatable<CacheKey>
        {
            public CacheKey(string query, JObject? variables)
            {
                Query = query;
                Variables = variables;
            }

            public string Query { get; }
            public JObject? Variables { get; }
            private static readonly JTokenEqualityComparer jtokenComparer = new JTokenEqualityComparer();

            public bool Equals([AllowNull] CacheKey? other)
                => other != null &&
                   Query == other.Query &&
                   jtokenComparer.Equals(Variables, other.Variables);

            public override bool Equals(object? obj)
                => Equals(obj as CacheKey);

            public override int GetHashCode()
                => HashCode.Combine(Query, jtokenComparer.GetHashCode(Variables));
        }

        public GraphQlController(ISchema schema, IDocumentExecuter executer, IEnumerable<IValidationRule> validationRules, ApplicationDbContext context, ILogger<GraphQlController> logger, IServiceProvider serviceProvider, IMemoryCache cache)
        {
            this.schema = schema;
            this.executer = executer;
            this.validationRules = validationRules;
            this.context = context;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.cache = cache;
        }

        [HttpPost]
        public Task<ExecutionResult> Post([BindRequired, FromBody] GraphQlPostBody body, CancellationToken cancellation)
        {
            if (!User.Identity.IsAuthenticated && body.Query.StartsWith("query", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Executing graphql query with caching");
                return cache.GetOrCreateAsync(
                    new CacheKey(body.Query, body.Variables),
                    entry =>
                    {
                        logger.LogInformation("Query not found in cache, calculating resultset");
                        entry.SlidingExpiration = CacheExpiration;
                        return Execute(body.Query, body.OperationName, body.Variables, cancellation);
                    });
            }
            else
            {
                logger.LogInformation("Executing graphql query without caching");
                return Execute(body.Query, body.OperationName, body.Variables, cancellation);
            }
        }

        [HttpGet]
        public Task<ExecutionResult> Get([FromQuery] GraphQlGetQuery getQuery, CancellationToken cancellation)
        {
            JObject? jsonVariables = ParseVariables(getQuery.Variables);
            if (!User.Identity.IsAuthenticated && getQuery.Query.StartsWith("query", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Executing graphql query with caching");
                return cache.GetOrCreateAsync(
                    new CacheKey(getQuery.Query, jsonVariables),
                    entry =>
                    {
                        logger.LogInformation("Query not found in cache, calculating resultset");
                        entry.SlidingExpiration = CacheExpiration;
                        return Execute(getQuery.Query, getQuery.OperationName, jsonVariables, cancellation);
                    });
            }
            else
            {
                logger.LogInformation("Executing graphql query without caching");
                return Execute(getQuery.Query, getQuery.OperationName, jsonVariables, cancellation);
            }
        }

        private static JObject? ParseVariables(string? variables)
        {
            if (variables == null)
            {
                return null;
            }

            try
            {
                return JObject.Parse(variables);
            }
            catch (Exception exception)
            {
                throw new Exception("Could not parse variables.", exception);
            }
        }

        private async Task<ExecutionResult> Execute(string query, string? operationName, JObject? variables, CancellationToken cancellation)
        {
            var options = new ExecutionOptions
            {
                Schema = schema,
                Query = query,
                OperationName = operationName,
                Inputs = variables?.ToInputs(),
                UserContext = new UserContext(User, context, serviceProvider),
                ComplexityConfiguration = new ComplexityConfiguration()
                {
                    MaxDepth = 20
                },
                ValidationRules = validationRules,
                CancellationToken = cancellation,
#if (DEBUG)
                ExposeExceptions = true,
                EnableMetrics = true,
#endif
            };

            logger.LogInformation("graphql: {0}, {1}", query, operationName);
            logger.LogDebug("variables: {0}", variables);

#if (DEBUG)
            DateTime start = DateTime.Now;
#endif
            ExecutionResult result = await executer.ExecuteAsync(options).ConfigureAwait(false);

#if (DEBUG)
            try
            {
                result.EnrichWithApolloTracing(start);
            }
            catch (Exception e)
            {
                logger.LogError(e, "error creating apollo trace");
            }
#endif

            if (result.Errors != null)
            {
                foreach (ExecutionError error in result.Errors)
                {
                    logger.LogError(error, "Error occurred while executing graphql");
                }
            }

            return result;
        }
    }
}
