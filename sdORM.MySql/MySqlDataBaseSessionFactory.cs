using System.Threading.Tasks;
using sdORM.Common;

namespace sdORM.MySql
{
    public class MySqlDataBaseSessionFactory : DataBaseSessionFactory
    {
        public MySqlDataBaseSessionFactory(SdOrmConfig config) 
            : base(config)
        {
        }
        
        public override IDataBaseSession CreateSession()
        {
            var session = new MySqlDataBaseSession(this._config);
            session.Connect();
            return session;
        }

        public override async Task<IDataBaseSessionAsync> CreateAsyncSession()
        {
            var session = new MySqlDataBaseSessionAsync(this._config);
            await session.ConnectAsync();
            return session;
        }
    }
}