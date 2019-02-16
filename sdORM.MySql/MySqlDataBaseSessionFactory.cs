using MySql.Data.MySqlClient;
using sdORM.Mapping;
using sdORM.Session.Implementations;
using sdORM.Session.Interfaces;

namespace sdORM.MySql
{
    public class MySqlDatabaseSessionFactory : DatabaseSessionFactory
    {
        public MySqlDatabaseSessionFactory(string connectionString, EntityMappingProvider mappingProvider)
            : base(connectionString, mappingProvider)
        {

        }

        protected override IDatabaseSession CreateSessionInternal()
        {
            return new DatabaseSession(new MySqlConnection(this.ConnectionString), this._mappingProvider, new MySqlSqlProvider());
        }

        protected override IDatabaseSessionAsync CreateAsyncSessionInternal()
        {
            return new DatabaseSessionAsync(new MySqlConnection(this.ConnectionString), this._mappingProvider, new MySqlSqlProvider());
        }

        protected override IRawDatabaseSession CreateRawSessionInternal()
        {
            return new RawDatabaseSession(new MySqlConnection(this.ConnectionString), this._mappingProvider, new MySqlSqlProvider());
        }

        protected override IRawDatabaseSessionAsync CreateRawSessionAsyncInternal()
        {
            return new RawDatabaseSessionAsync(new MySqlConnection(this.ConnectionString), this._mappingProvider, new MySqlSqlProvider());
        }
    }
}