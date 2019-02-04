using System;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Global

namespace sdORM.Helper
{
    public static class Guard
    {
        public static void NotNull(object @object, string propertyName)
        {
            if(@object == null)
                throw new ArgumentNullException($"{propertyName} is null");
        }
    }
}