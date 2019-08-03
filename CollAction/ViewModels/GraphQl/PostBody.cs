using Newtonsoft.Json.Linq;

namespace CollAction.ViewModels.GraphQl
{
    public class PostBody
    {
        public string OperationName { get; set; }

        public string Query { get; set; }

        public JObject Variables { get; set; }
    }
}
