using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CollAction.ViewModels.GraphQl
{
    public sealed class GraphQlPostBody
    {
        [JsonProperty("operationName")]
        public string? OperationName { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; } = null!;

        [JsonProperty("variables")]
        public JObject? Variables { get; set; }
    }
}
