using System;

namespace sdORM.Common
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