using System;
using System.Collections.Generic;

namespace sdORM.Mapping
{
    public class EntityMappingProvider
    {
        private Dictionary<Type, object> ExistingMappings { get; }

        // not sure if I'm happy with this...
        private readonly Type _entityMappingType;

        public EntityMappingProvider(Type entityMappingType)
        {
            this._entityMappingType = entityMappingType;
            this.ExistingMappings = new Dictionary<Type, object>();
        }

        public EntityMapping<T> GetMapping<T>()
        {
            if (this.ExistingMappings.ContainsKey(typeof(T)))
                return (EntityMapping<T>)this.ExistingMappings[typeof(T)];

            var mapping = this.GenerateEntityMappingForType<T>(typeof(T));

            mapping.Map();

            this.ExistingMappings.Add(typeof(T), mapping);

            return mapping;
        }
        
        private EntityMapping<T> GenerateEntityMappingForType<T>(Type type)
        {
            var mappingType = this._entityMappingType.MakeGenericType(type);
            return (EntityMapping<T>)Activator.CreateInstance(mappingType);
        }
    }
}