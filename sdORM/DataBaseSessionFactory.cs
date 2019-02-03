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

        /// <summary>
        /// Will create a <see cref="IDataBaseSession"/> with the spcified connection and open it.
        /// </summary>
        /// <returns>The created databasesession.</returns>
        public abstract IDataBaseSession CreateSession();

        /// <summary>
        /// Will asynchronously create a <see cref="IDataBaseSessionAsync"/> with the spcified connection and open it.
        /// </summary>
        /// <returns>The created databasesession.</returns>
        public abstract Task<IDataBaseSessionAsync> CreateAsyncSession();
    }
}