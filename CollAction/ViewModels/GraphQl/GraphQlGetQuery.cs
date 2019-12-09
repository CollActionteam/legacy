using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.GraphQl
{
    public class GraphQlGetQuery
    {
        [Required]
        public string Query { get; set; }

        public string OperationName { get; set; }

        public string Variables { get; set; }
    }
}
