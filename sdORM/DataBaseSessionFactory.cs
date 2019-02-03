using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Session;

namespace sdORM
{
    public abstract class DataBaseSessionFactory
    {
        protected readonly SdOrmConfig _config;

        protected DataBaseSessionFactory(SdOrmConfig config)
        {
            this._config = config;
        }

        public async Task InitializeAsync()
        {
        }

        public abstract IDataBaseSession CreateSession();
        public abstract Task<IDataBaseSessionAsync> CreateAsyncSession();
    }
}