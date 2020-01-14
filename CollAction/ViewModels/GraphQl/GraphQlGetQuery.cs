using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ViewModels.GraphQl
{
    public class GraphQlGetQuery
    {
        [FromQuery(Name = "query")]
        public string Query { get; set; }

        [FromQuery(Name = "operationName")]
        public string? OperationName { get; set; }

        [FromQuery(Name = "variables")]
        public string? Variables { get; set; }
    }
}
