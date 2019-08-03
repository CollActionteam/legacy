using CollAction.Data;
using CollAction.GraphQl;
using GraphQL;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Route("graphql")]
    [ApiController]
    public class GraphQlController : Controller
    {
        private readonly IDocumentExecuter executer;
        private readonly IValidationRule authorizationValidationRule;
        private readonly ApplicationDbContext context;
        private readonly ISchema schema;

        public GraphQlController(ISchema schema, IDocumentExecuter executer, IValidationRule authorizationValidationRule, ApplicationDbContext context)
        {
            this.schema = schema;
            this.executer = executer;
            this.authorizationValidationRule = authorizationValidationRule;
            this.context = context;
        }

        [HttpPost]
        public Task<ExecutionResult> Post(
            [BindRequired, FromBody] PostBody body,
            CancellationToken cancellation)
        {
            return Execute(body.Query, body.OperationName, body.Variables, cancellation);
        }

        [HttpGet]
        public Task<ExecutionResult> Get(
            [FromQuery] string query,
            [FromQuery] string variables,
            [FromQuery] string operationName,
            CancellationToken cancellation)
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
            var validationRules = DocumentValidator.CoreRules();
            validationRules.Add(authorizationValidationRule);
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

        public class PostBody
        {
            public string OperationName { get; set; }

            public string Query { get; set; }

            public JObject Variables { get; set; }
        }
    }
}
