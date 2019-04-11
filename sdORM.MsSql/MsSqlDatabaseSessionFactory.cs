using System.Data.SqlClient;
using sdORM.Mapping;
using sdORM.Session.Implementations;
using sdORM.Session.Interfaces;

namespace sdORM.MsSql
{
    public class MsSqlDatabaseSessionFactory : DatabaseSessionFactory
    {
        public MsSqlDatabaseSessionFactory(string connectionString, EntityMappingProvider mappingProvider) 
            : base(connectionString, mappingProvider)
        {
        }

        protected override IDatabaseSession CreateSessionInternal()
        {
            return new DatabaseSession(new SqlConnection(this.ConnectionString), this._mappingProvider, new MsSqlSqlProvider());
        }

        protected override IDatabaseSessionAsync CreateAsyncSessionInternal()
        {
            return new DatabaseSessionAsync(new SqlConnection(this.ConnectionString), this._mappingProvider, new MsSqlSqlProvider());
        }

        protected override IRawDatabaseSession CreateRawSessionInternal()
        {
            return new RawDatabaseSession(new SqlConnection(this.ConnectionString), this._mappingProvider, new MsSqlSqlProvider());
        }

        protected override IRawDatabaseSessionAsync CreateRawSessionAsyncInternal()
        {
            return new RawDatabaseSessionAsync(new SqlConnection(this.ConnectionString), this._mappingProvider, new MsSqlSqlProvider());
        }
    }
}