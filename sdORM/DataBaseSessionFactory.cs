﻿using System.Threading.Tasks;
using sdORM.Helper;
using sdORM.Mapping;
using sdORM.Session.Exceptions;
using sdORM.Session.Interfaces;

namespace sdORM
{
    public abstract class DatabaseSessionFactory
    {
        protected readonly string ConnectionString;
        protected readonly EntityMappingProvider _mappingProvider;

        public bool IsInitialized { get; private set; }

        protected DatabaseSessionFactory(string connectionString, EntityMappingProvider mappingProvider)
        {
            Guard.NotNull(connectionString, nameof(connectionString));
            Guard.NotNull(mappingProvider, nameof(mappingProvider));

            this.ConnectionString = connectionString;
            this._mappingProvider = mappingProvider;
        }

        #region Initialization

        /// <summary>
        /// Will initialize the factory, making sure everything is ready for sessions to be created.
        /// </summary>
        public void Initialize()
        {
            using (var session = this.CreateSessionInternal())
            {
                session.Connect();

                this._mappingProvider.ValidateMappingsAgainstDatabase(session);
            }

            this.InitializeInternal();

            this.IsInitialized = true;
        }

        protected virtual void InitializeInternal()
        {

        }

        /// <summary>
        /// The async version of <see cref="Initialize"/>.
        /// </summary>
        public async Task InitializeAsync()
        {
            using (var session = this.CreateAsyncSessionInternal())
            {
                await session.ConnectAsync();

                await this._mappingProvider.ValidateMappingsAgainstDatabaseAsync(session);
            }

            await this.InitializeInternalAsync();

            this.IsInitialized = true;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected virtual async Task InitializeInternalAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {

        }

        #endregion

        #region Sessions

        #region IDatabaseSession

        /// <summary>
        /// Will create a <see cref="IDatabaseSession"/> with the specified connection and open it.
        /// </summary>
        /// <returns>The created Databasesession.</returns>
        public IDatabaseSession CreateSession()
        {
            if (this.IsInitialized == false)
                throw new DatabaseSessionFactoryNotInitializedException(this.GetType());

            var session = this.CreateSessionInternal();
            session.Connect();
            return session;
        }

        protected abstract IDatabaseSession CreateSessionInternal();

        #endregion

        #region IDatabaseSessionAsync

        /// <summary>
        /// Will asynchronously create a <see cref="IDatabaseSessionAsync"/> with the specified connection and open it.
        /// </summary>
        /// <returns>The created Databasesession.</returns>
        public async Task<IDatabaseSessionAsync> CreateAsyncSession()
        {
            if (this.IsInitialized == false)
                throw new DatabaseSessionFactoryNotInitializedException(this.GetType());

            var session = this.CreateAsyncSessionInternal();
            await session.ConnectAsync();
            return session;
        }

        protected abstract IDatabaseSessionAsync CreateAsyncSessionInternal();

        #endregion

        #region IRawDatabaseSession

        public IRawDatabaseSession CreateRawSession()
        {
            if (this.IsInitialized == false)
                throw new DatabaseSessionFactoryNotInitializedException(this.GetType());

            var session = this.CreateRawSessionInternal();
            session.Connect();
            return session;
        }

        protected abstract IRawDatabaseSession CreateRawSessionInternal();

        #endregion

        #region IRawDatabaseSessionAsync

        public async Task<IRawDatabaseSessionAsync> CreateRawSessionAsync()
        {
            if (this.IsInitialized == false)
                throw new DatabaseSessionFactoryNotInitializedException(this.GetType());

            var session = this.CreateRawSessionAsyncInternal();
            await session.ConnectAsync();
            return session;
        }

        protected abstract IRawDatabaseSessionAsync CreateRawSessionAsyncInternal();

        #endregion

        #endregion
    }
}