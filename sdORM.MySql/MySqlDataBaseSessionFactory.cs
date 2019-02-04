using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using sdORM.Common;
using sdORM.Session;
using sdORM.Session.Exceptions;

namespace sdORM.MySql
{
    public class MySqlDataBaseSessionFactory : DataBaseSessionFactory
    {
        private MySqlConnection _connection;

        public MySqlDataBaseSessionFactory(SdOrmConfig config)
            : base(config)
        {
        }

        public override void Initalize()
        {
            this._connection = new MySqlConnection(this._config.ConnectionString);

            this.IsInitialized = true;

            using (var session = this.CreateSession())
            {
                this._config.Mappings.ValidateMappingsAgainstDatabase(session);
            }
        }

        public override async Task InitializeAsync()
        {
            this._connection = new MySqlConnection(this._config.ConnectionString);

            this.IsInitialized = true;

            using (var session = await this.CreateAsyncSession())
            {
                await this._config.Mappings.ValidateMappingsAgainstDatabaseAsync(session);
            }
        }

        public override IDataBaseSession CreateSession()
        {
            if (this.IsInitialized == false)
                throw new DataBaseSessionFactoryNotInitializedException(this.GetType());

            var session = new DataBaseSession(this._connection, this._config.Mappings, new MySqlSqlProvider());
            session.Connect();
            return session;
        }

        public override async Task<IDataBaseSessionAsync> CreateAsyncSession()
        {
            if (this.IsInitialized == false)
                throw new DataBaseSessionFactoryNotInitializedException(this.GetType());

            var session = new DataBaseSessionAsync(this._connection, this._config.Mappings, new MySqlSqlProvider());
            await session.ConnectAsync();
            return session;
        }
    }
}