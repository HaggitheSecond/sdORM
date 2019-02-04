using System.Data.Common;
using sdORM.Common;
using sdORM.Helper;
using sdORM.Mapping;

namespace sdORM.Session
{
    public abstract class DatabaseSessionBase
    {
        protected DbConnection Connection { get; }
        protected ISqlSpecifcProvider SqlSpecifcProvider { get; }
        protected EntityMappingProvider EntityMappingProvider { get; }

        protected DatabaseSessionBase(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecifcProvider sqlSpecifcProvider)
        {
            Guard.NotNull(connection, nameof(connection));
            Guard.NotNull(entityMappingProvider, nameof(entityMappingProvider));
            Guard.NotNull(sqlSpecifcProvider, nameof(sqlSpecifcProvider));

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