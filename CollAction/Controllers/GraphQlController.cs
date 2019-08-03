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
        private readonly ISchema schema;

        public GraphQlController(ISchema schema, IDocumentExecuter executer, IValidationRule authorizationValidationRule)
        {
            this.schema = schema;
            this.executer = executer;
            this.authorizationValidationRule = authorizationValidationRule;
        }

        [HttpPost]
        public Task<ExecutionResult> Post(
            [BindRequired, FromBody] PostBody body,
            [FromServices] ApplicationDbContext dbContext,
            CancellationToken cancellation)
        {
            return Execute(dbContext, body.Query, body.OperationName, body.Variables, cancellation);
        }

        public class PostBody
        {
            public string OperationName;
            public string Query;
            public JObject Variables;
        }

        [HttpGet]
        public Task<ExecutionResult> Get(
            [FromQuery] string query,
            [FromQuery] string variables,
            [FromQuery] string operationName,
            [FromServices] ApplicationDbContext dbContext,
            CancellationToken cancellation)
        {
            var jObject = ParseVariables(variables);
            return Execute(dbContext, query, operationName, jObject, cancellation);
        }

        private Task<ExecutionResult> Execute(
            ApplicationDbContext dbContext,
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
                    Context = dbContext,
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
    }
}
