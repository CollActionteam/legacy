using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.GraphQl
{
    public sealed class GraphQlGetQuery
    {
        [FromQuery(Name = "query")]
        [Required]
        public string Query { get; set; } = null!;

        [FromQuery(Name = "operationName")]
        public string? OperationName { get; set; }

        [FromQuery(Name = "variables")]
        public string? Variables { get; set; }
    }
}
