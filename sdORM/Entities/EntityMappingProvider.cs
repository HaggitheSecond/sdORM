using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sdORM.Interfaces;

namespace sdORM.Entities
{
    public class EntityMappingProvider
    {
        private readonly IDataBaseSession _session;

        private Dictionary<Type, EntityMapping> ExistingMappings { get; }

        public EntityMappingProvider(IDataBaseSession session)
        {
            this._session = session;

            this.ExistingMappings = new Dictionary<Type, EntityMapping>();
        }

        public EntityMapping<T> GetMapping<T>()
        {
            if (this.ExistingMappings.ContainsKey(typeof(T)))
                return (EntityMapping<T>)this.ExistingMappings[typeof(T)];
            
            var mapping = new EntityMapping<T>(this._session);

            mapping.Map();

            this.ExistingMappings.Add(typeof(T), mapping);

            return mapping;
        }

        public void PreGenerateEntityMappings(IList<Type> entityTypes)
        {
            foreach (var currentEntityType in entityTypes)
            {
                var mapping = this.GenerateEntityMappingForType(currentEntityType);

                mapping.Map();

                this.ExistingMappings.Add(currentEntityType, mapping);
            }
        }

        public async Task PreGenerateEntityMappingsAsync(IList<Type> entityTypes)
        {
            foreach (var currentEntityType in entityTypes)
            {
                var mapping = this.GenerateEntityMappingForType(currentEntityType);

                await mapping.MapAsync();

                this.ExistingMappings.Add(currentEntityType, mapping);
            }
        }

        private EntityMapping GenerateEntityMappingForType(Type type)
        {
            var mappingType = typeof(EntityMapping<>).MakeGenericType(type);
            return (EntityMapping)Activator.CreateInstance(mappingType, this._session);
        }
    }
}