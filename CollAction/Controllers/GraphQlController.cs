using CollAction.Data;
using CollAction.GraphQl;
using CollAction.ViewModels.GraphQl;
using GraphQL;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly ISchema schema;

        public GraphQlController(ISchema schema, IDocumentExecuter executer, IEnumerable<IValidationRule> validationRules, ApplicationDbContext context)
        {
            this.schema = schema;
            this.executer = executer;
            this.validationRules = validationRules;
            this.context = context;
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

        private Task<ExecutionResult> Execute(
            string query,
            string operationName,
            JObject variables,
            CancellationToken cancellation)
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
                    User = User
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

            return executer.ExecuteAsync(options);
        }
    }
}
