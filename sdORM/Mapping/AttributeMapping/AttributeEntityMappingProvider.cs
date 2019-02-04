using System;

namespace sdORM.Mapping.AttributeMapping
{
    public class AttributeEntityMappingProvider : EntityMappingProvider
    {
        protected override EntityMapping CreateGenericEntityMappingInstance(Type type)
        {
            return (EntityMapping)Activator.CreateInstance(typeof(AttributeEntityMapping<>).MakeGenericType(type));
        }
    }
}