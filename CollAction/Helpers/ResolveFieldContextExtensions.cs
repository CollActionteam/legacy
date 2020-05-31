using CollAction.GraphQl;
using GraphQL.Types;

namespace CollAction.Helpers
{
    public static class ResolveFieldContextExtensions
    {
        public static UserContext GetUserContext<T>(this ResolveFieldContext<T> context)
            => (UserContext)context.UserContext;

        public static UserContext GetUserContext(this ResolveFieldContext context)
            => (UserContext)context.UserContext;
    }
}
