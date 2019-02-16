using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Session
{
    public interface IDatabaseSession : IDisposable
    {
        /// <summary>
        /// Creates the connection. This should never be used if you're using the DatabaseSessionFactory.
        /// </summary>
        void Connect();

        /// <summary>
        /// Creates an transaction which will be applied to all non-reading operations and will be commited when the session is disposed.
        /// </summary>
        void AddTransaction();

        /// <summary>
        /// Queries the Database for ALL entities in the table.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <returns>All entities in the table.</returns>
        IList<T> Query<T>() where T : new();

        /// <summary>
        /// Queries the Database for all entities that match the predicate in the table.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <param name="predicate">The predicate by which the entities will be filtered.</param>
        /// <returns>The matching entities in the table.</returns>
        IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : new();

        /// <summary>
        /// Queries the Database for all entities with the specified sql query.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <param name="sql">The raw sql by which the table will be queried.</param>
        /// <returns>The matching entities in the table.</returns>
        IList<T> Query<T>(ParameterizedSql sql) where T : new();

        /// <summary>
        /// Gets the entity with the matching primarykey-value or null if none exists.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <param name="id">The primarykey value.</param>
        /// <returns>The entity with the matching id or null if none exists.</returns>
        T GetByID<T>(object id) where T : new();

        /// <summary>
        /// Will either save or update the entity depending on the primary key value.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>The entity with (potentially) updated properties.</returns>
        T SaveOrUpdate<T>(T entity);

        /// <summary>
        /// Saves a new entity. Will throw an exeception if entity is already saved in the Database.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>The entity with (potentially) updated properties.</returns>
        /// <exception cref="ArgumentException">Thrown when the entity is already saved.</exception>
        T Save<T>(T entity);

        /// <summary>
        /// Updates an already existing entity. Will throw an exeception if entity is not yet saved.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>The entity with (potentially) updated properties.</returns>
        /// <exception cref="ArgumentException">Thrown when the entity is not yet saved.</exception>
        T Update<T>(T entity);

        /// <summary>
        /// Deletes the entity specified by the id.
        /// </summary>
        /// <typeparam name="T">The entity whose table should be queried.</typeparam>
        /// <param name="id">The id of the entity to be deleted.</param>
        void Delete<T>(object id);

        /// <summary>
        /// Gets metadata for the specified table or null if table does not exist.
        /// </summary>
        /// <param name="tableName">The tableName.</param>
        /// <returns>The metadata for the table or null if table does not exist.</returns>
        TableMetaData GetTableMetaData(string tableName);
    }
}