using System.Collections.Generic;

namespace sdORM.Extensions
{
    public static class IListExtensions
    {
        public static IList<T> EmptyIfNull<T>(this IList<T> self)
        {
            return self ?? new List<T>();
        }
    }
}