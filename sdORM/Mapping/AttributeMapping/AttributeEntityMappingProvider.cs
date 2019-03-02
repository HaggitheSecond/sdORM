using System;

namespace sdORM.Mapping.AttributeMapping
{
    public class AttributeEntityMappingProvider : EntityMappingProvider
    {
        public AttributeEntityMappingProvider(ITypeToColumnTypeConverter converter)
            : base(converter)
        {

        }

        protected override EntityMapping CreateGenericEntityMappingInstance(Type type)
        {
            return (EntityMapping)Activator.CreateInstance(typeof(AttributeEntityMapping<>).MakeGenericType(type));
        }
    }
}