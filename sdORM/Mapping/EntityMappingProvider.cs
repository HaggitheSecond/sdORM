﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using sdORM.Mapping.AttributeMapping.Attributes.Entities;
using sdORM.Mapping.Exceptions;
using sdORM.Session;
using sdORM.Session.Interfaces;

namespace sdORM.Mapping
{
    public abstract class EntityMappingProvider
    {
        private readonly ITypeToColumnTypeConverter _converter;
        private readonly Dictionary<Type, EntityMapping> _mappings;

        protected EntityMappingProvider(ITypeToColumnTypeConverter converter)
        {
            this._converter = converter;
            this._mappings =  new Dictionary<Type, EntityMapping>();
        }

        public EntityMapping<T> GetMapping<T>()
        {
            return this._mappings.ContainsKey(typeof(T))
                ? (EntityMapping<T>)this._mappings[typeof(T)]
                : throw new TypeNotMappedException(typeof(T));
        }

        #region Validation

        public void ValidateMappingsAgainstDatabase(IDatabaseSession session)
        {
            foreach (var currentMapping in this._mappings)
            {
                currentMapping.Value.ValidateAgainstDatabase(session, this._converter);
            }
        }

        public async Task ValidateMappingsAgainstDatabaseAsync(IDatabaseSessionAsync session)
        {
            foreach (var currentMapping in this._mappings)
            {
                await currentMapping.Value.ValidateAgainstDatabase(session, this._converter);
            }
        }

        #endregion

        #region Internal

        protected abstract EntityMapping CreateGenericEntityMappingInstance(Type type);
        private void AddMapping(Type type, EntityMapping entityMapping)
        {
            entityMapping.ValidateAndMap();

            this._mappings.Add(type, entityMapping);
        }

        #endregion

        #region AddMappingFromType

        public void AddMapping(Type type)
        {
            this.AddMapping(type, this.CreateGenericEntityMappingInstance(type));
        }

        public void AddMappings(params Type[] @params)
        {
            foreach (var currentType in @params)
            {
                this.AddMapping(currentType);
            }
        }

        #endregion

        #region AddMappingsFromAssembly

        public void AddMappingsFromAssembly(string assemblyString)
        {
            this.AddMappingsFromAssembly(Assembly.Load(assemblyString));
        }

        public void AddMappingsFromAssembly(Type typeInAssembly)
        {
            this.AddMappingsFromAssembly(Assembly.GetAssembly(typeInAssembly));
        }

        public void AddMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();

            foreach (var currentType in types)
            {
                if (currentType.GetCustomAttribute<DBEntityAttribute>() == null)
                    continue;

                this.AddMapping(currentType);
            }
        }

        #endregion
    }
}