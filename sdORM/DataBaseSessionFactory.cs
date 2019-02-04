using System.Data.Common;
using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Mapping;
using sdORM.Session;

namespace sdORM
{
    public abstract class DataBaseSessionFactory
    {
        protected readonly DbConnection _connection;
        protected readonly EntityMappingProvider _mappingProvider;
        
        public bool IsInitialized { get; protected set; }

        public DataBaseSessionFactory(DbConnection connection, EntityMappingProvider mappingProvider)
        {
            this._connection = connection;
            this._mappingProvider = mappingProvider;
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