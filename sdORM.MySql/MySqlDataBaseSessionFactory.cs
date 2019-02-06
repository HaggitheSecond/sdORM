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

        public override void Initalize()
        {
            this.IsInitialized = true;

            using (var session = this.CreateSession())
            {
                this._mappingProvider.ValidateMappingsAgainstDatabase(session);
            }
        }

        public override async Task InitializeAsync()
        {
            this.IsInitialized = true;

            using (var session = await this.CreateAsyncSession())
            {
                await this._mappingProvider.ValidateMappingsAgainstDatabaseAsync(session);
            }
        }

        public override IDatabaseSession CreateSession()
        {
            if (this.IsInitialized == false)
                throw new DatabaseSessionFactoryNotInitializedException(this.GetType());

            var session = new DatabaseSession(new MySqlConnection(this.ConnectionString), this._mappingProvider, new MySqlSqlProvider());
            session.Connect();
            return session;
        }

        public override async Task<IDatabaseSessionAsync> CreateAsyncSession()
        {
            if (this.IsInitialized == false)
                throw new DatabaseSessionFactoryNotInitializedException(this.GetType());

            var session = new DatabaseSessionAsync(new MySqlConnection(this.ConnectionString), this._mappingProvider, new MySqlSqlProvider());
            await session.ConnectAsync();
            return session;
        }
    }
}