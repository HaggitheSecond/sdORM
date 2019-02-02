using System;
using System.Collections.Generic;
using System.Linq;

namespace sdORM.Extensions
{
    public static class LINQExtensions
    {
        public static bool None<T>(this IEnumerable<T> self, Func<T, bool> predicate = null)
        {
            return predicate == null ? self.Any() == false : self.Any(predicate) == false;
        }
    }
}