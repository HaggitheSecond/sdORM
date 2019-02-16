using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Session.Interfaces
{
    public interface IDatabaseSessionAsync : IDisposable, IDatabaseSessionWithTransaction
    {
        /// <summary>
        /// The async version of the Connect() method in IDatabaseSession.
        /// <seealso cref="IDatabaseSession"/>
        /// </summary>
        Task ConnectAsync();
        
        /// <summary>
        /// The async version of <see cref="IDatabaseSession.Query{T}()"/>.
        /// </summary>
        Task<IList<T>> QueryAsync<T>() where T : new();

        /// <summary>
        /// The async version of <see cref="IDatabaseSession.Query{T}()"/>.
        /// </summary>
        Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : new();
        
        /// <summary>
        /// The async version of <see cref="IDatabaseSession.GetByID{T}"/>.
        /// </summary>
        Task<T> GetByIDAsync<T>(object id) where T : new();

        /// <summary>
        /// The async version of <see cref="IDatabaseSession.SaveOrUpdate{T}"/>.
        /// </summary>
        Task<T> SaveOrUpdateAsync<T>(T entity);

        /// <summary>
        /// The async version of <see cref="IDatabaseSession.Save{T}"/>.
        /// </summary>
        Task<T> SaveAsync<T>(T entity);

        /// <summary>
        /// The async version of <see cref="IDatabaseSession.Update{T}"/>.
        /// </summary>
        Task<T> UpdateAsync<T>(T entity);

        /// <summary>
        /// The async version of <see cref="IDatabaseSession.Delete{T}"/>.
        /// </summary>
        Task Delete<T>(object id);

        /// <summary>
        /// The async version of <see cref="IDatabaseSession.GetTableMetaData{T}"/>.
        /// </summary>
        Task<TableMetaData> GetTableMetaDataAsync(string tableName);
    }
}