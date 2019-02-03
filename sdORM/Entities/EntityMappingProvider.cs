using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sdORM.Interfaces;

namespace sdORM.Entities
{
    public class EntityMappingProvider
    {
        private Dictionary<Type, EntityMapping> ExistingMappings { get; }

        // not sure if I'm happy with this...
        private readonly Type _entityMappingType;

        public EntityMappingProvider(Type entityMappingType)
        {
            this._entityMappingType = entityMappingType;
            this.ExistingMappings = new Dictionary<Type, EntityMapping>();
        }

        public EntityMapping<T> GetMapping<T>()
        {
            if (this.ExistingMappings.ContainsKey(typeof(T)))
                return (EntityMapping<T>)this.ExistingMappings[typeof(T)];

            var mapping = this.GenerateEntityMappingForType(typeof(T));

            mapping.Map();

            this.ExistingMappings.Add(typeof(T), mapping);

            return (EntityMapping<T>)mapping;
        }
        
        private EntityMapping GenerateEntityMappingForType(Type type)
        {
            var mappingType = this._entityMappingType.MakeGenericType(type);
            return (EntityMapping)Activator.CreateInstance(mappingType);
        }
    }
}