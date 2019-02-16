using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using sdORM.Common;
using sdORM.Mapping;
using sdORM.Session;
using sdORM.Session.Exceptions;

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
    }
}