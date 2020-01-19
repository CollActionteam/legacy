using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CollAction.Helpers
{
    public class LambdaEqualityComparer<TClass, TProp> : IEqualityComparer<TClass>
    {
        private readonly Func<TClass, TProp> getProp;

        public LambdaEqualityComparer(Func<TClass, TProp> getProp)
        {
            this.getProp = getProp;
        }

        public bool Equals([AllowNull] TClass x, [AllowNull] TClass y)
            => (x == null && y == null) || (getProp(x)?.Equals(getProp(y)) ?? false);

        public int GetHashCode([DisallowNull] TClass obj)
            => getProp(obj)?.GetHashCode() ?? -1;
    }
}
