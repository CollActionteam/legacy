using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
