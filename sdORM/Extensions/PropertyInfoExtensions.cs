using System;

namespace sdORM.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool IsNullable<T>(this T _)
        {
            return Nullable.GetUnderlyingType(typeof(T)) != null;
        }
    }
}