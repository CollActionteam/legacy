using Newtonsoft.Json.Linq;

namespace CollAction.ViewModels.GraphQl
{
    public class GraphQlPostBody
    {
        public string OperationName { get; set; }

        public string Query { get; set; }

        public JObject Variables { get; set; }
    }
}
