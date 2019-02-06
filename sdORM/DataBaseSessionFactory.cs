using System.Data.Common;
using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Helper;
using sdORM.Mapping;
using sdORM.Session;

namespace sdORM
{
    public abstract class DatabaseSessionFactory
    {
        protected readonly string ConnectionString;
        protected readonly EntityMappingProvider _mappingProvider;
        
        public bool IsInitialized { get; protected set; }

        protected DatabaseSessionFactory(string connectionString, EntityMappingProvider mappingProvider)
        {
            Guard.NotNull(connectionString, nameof(connectionString));
            Guard.NotNull(mappingProvider, nameof(mappingProvider));

            this.ConnectionString = connectionString;
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
        /// Will create a <see cref="IDatabaseSession"/> with the specified connection and open it.
        /// </summary>
        /// <returns>The created Databasesession.</returns>
        public abstract IDatabaseSession CreateSession();

        /// <summary>
        /// Will asynchronously create a <see cref="IDatabaseSessionAsync"/> with the specified connection and open it.
        /// </summary>
        /// <returns>The created Databasesession.</returns>
        public abstract Task<IDatabaseSessionAsync> CreateAsyncSession();
    }
}