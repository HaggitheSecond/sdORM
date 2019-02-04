using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using sdORM.Common;
using sdORM.Mapping;
using sdORM.Session;
using sdORM.Session.Exceptions;

namespace sdORM.MySql
{
    public class MySqlDataBaseSessionFactory : DataBaseSessionFactory
    {
        public MySqlDataBaseSessionFactory(string connectionString, EntityMappingProvider mappingProvider)
            : base(new MySqlConnection(connectionString), mappingProvider)
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

        public override IDataBaseSession CreateSession()
        {
            if (this.IsInitialized == false)
                throw new DataBaseSessionFactoryNotInitializedException(this.GetType());

            var session = new DataBaseSession(this._connection, this._mappingProvider, new MySqlSqlProvider());
            session.Connect();
            return session;
        }

        public override async Task<IDataBaseSessionAsync> CreateAsyncSession()
        {
            if (this.IsInitialized == false)
                throw new DataBaseSessionFactoryNotInitializedException(this.GetType());

            var session = new DataBaseSessionAsync(this._connection, this._mappingProvider, new MySqlSqlProvider());
            await session.ConnectAsync();
            return session;
        }
    }
}