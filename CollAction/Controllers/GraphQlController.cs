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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("graphql")]
    [ApiController]
    public class GraphQlController : Controller
    {
        private readonly IDocumentExecuter executer;
        private readonly IEnumerable<IValidationRule> validationRules;
        private readonly ApplicationDbContext context;
        private readonly ILogger<GraphQlController> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly ISchema schema;

        public GraphQlController(ISchema schema, IDocumentExecuter executer, IEnumerable<IValidationRule> validationRules, ApplicationDbContext context, ILogger<GraphQlController> logger, IServiceProvider serviceProvider)
        {
            this.schema = schema;
            this.executer = executer;
            this.validationRules = validationRules;
            this.context = context;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        [HttpPost]
        public Task<ExecutionResult> Post([BindRequired, FromBody] PostBody body, CancellationToken cancellation)
        {
            return Execute(body.Query, body.OperationName, body.Variables, cancellation);
        }

        [HttpGet]
        public Task<ExecutionResult> Get([FromQuery] string query, [FromQuery] string variables, [FromQuery] string operationName, CancellationToken cancellation)
        {
            var jsonVariables = ParseVariables(variables);
            return Execute(query, operationName, jsonVariables, cancellation);
        }

        private static JObject ParseVariables(string variables)
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

        private async Task<ExecutionResult> Execute(string query, string operationName, JObject variables, CancellationToken cancellation)
        {
            var options = new ExecutionOptions
            {
                Schema = schema,
                Query = query,
                OperationName = operationName,
                Inputs = variables?.ToInputs(),
                UserContext = new UserContext()
                {
                    Context = context,
                    User = User,
                    ServiceProvider = serviceProvider
                },
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
            ExecutionResult result = await executer.ExecuteAsync(options);

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
