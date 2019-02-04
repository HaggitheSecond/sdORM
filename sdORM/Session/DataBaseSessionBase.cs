using System.Data.Common;
using sdORM.Common;
using sdORM.Mapping;

namespace sdORM.Session
{
    public abstract class DataBaseSessionBase
    {
        protected DbConnection Connection { get; }
        protected ISqlSpecifcProvider SqlSpecifcProvider { get; }
        protected EntityMappingProvider EntityMappingProvider { get; }

        protected DataBaseSessionBase(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecifcProvider sqlSpecifcProvider)
        {
            Guard.NotNull(connection, nameof(connection));
            Guard.NotNull(entityMappingProvider, nameof(entityMappingProvider));

            this.Connection = connection;

            this.SqlSpecifcProvider = sqlSpecifcProvider;
            this.EntityMappingProvider = entityMappingProvider;
        }

        public void Dispose()
        {
            this.Connection?.Dispose();
        }
    }
}