using System;

namespace sdORM.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool IsNullable<T>(this T self)
        {
            return Nullable.GetUnderlyingType(typeof(T)) != null;
        }
    }
}