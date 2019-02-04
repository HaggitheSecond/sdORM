using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Session;

namespace sdORM
{
    public abstract class DataBaseSessionFactory
    {
        protected readonly SdOrmConfig _config;

        public bool IsInitialized { get; protected set; }

        protected DataBaseSessionFactory(SdOrmConfig config)
        {
            this._config = config;
        }

        /// <summary>
        /// Will initialize the factory, making sure everything is ready for sessions to be created.
        /// </summary>
        public abstract void Initalize();

        /// <summary>
        /// The async version of <see cref="Initalize"/>.
        /// </summary>
        public abstract Task InitializeAsync();
        
        /// <summary>
        /// Will create a <see cref="IDataBaseSession"/> with the specified connection and open it.
        /// </summary>
        /// <returns>The created databasesession.</returns>
        public abstract IDataBaseSession CreateSession();

        /// <summary>
        /// Will asynchronously create a <see cref="IDataBaseSessionAsync"/> with the specified connection and open it.
        /// </summary>
        /// <returns>The created databasesession.</returns>
        public abstract Task<IDataBaseSessionAsync> CreateAsyncSession();
    }
}