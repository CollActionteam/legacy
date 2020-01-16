using GraphQL.Types;
using System.ComponentModel.DataAnnotations;

namespace CollAction.GraphQl.Mutations.Result
{
    public sealed class ValidationResultGraph : ObjectGraphType<ValidationResult>
    {
        public ValidationResultGraph()
        {
            Field(x => x.ErrorMessage);
            Field(x => x.MemberNames);
        }
    }
}
