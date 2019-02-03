using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Session
{
    public interface IDataBaseSessionAsync : IDisposable
    {
        /// <summary>
        /// The async version of the Connect() method in IDataBaseSession.
        /// <seealso cref="IDataBaseSession"/>
        /// </summary>
        Task ConnectAsync();
        /// <summary>
        /// The async version of <see cref="IDataBaseSession.Query{T}()"/>.
        /// </summary>
        Task<IList<T>> QueryAsync<T>() where T : new();
        /// <summary>
        /// The async version of <see cref="IDataBaseSession.Query{T}()"/>.
        /// </summary>
        Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : new();
        /// <summary>
        /// The async version of <see cref="IDataBaseSession.Query{T}(ParameterizedSql)"/>.
        /// </summary>
        Task<IList<T>> QueryAsync<T>(ParameterizedSql sql) where T : new();

        /// <summary>
        /// The async version of <see cref="IDataBaseSession.GetByID{T}"/>.
        /// </summary>
        Task<T> GetByIDAsync<T>(object id) where T : new();

        /// <summary>
        /// The async version of <see cref="IDataBaseSession.SaveOrUpdate{T}"/>.
        /// </summary>
        Task<T> SaveOrUpdateAsync<T>(T entity);

        /// <summary>
        /// The async version of <see cref="IDataBaseSession.Save{T}"/>.
        /// </summary>
        Task<T> SaveAsync<T>(T entity);

        /// <summary>
        /// The async version of <see cref="IDataBaseSession.Update{T}"/>.
        /// </summary>
        Task<T> UpdateAsync<T>(T entity);

        /// <summary>
        /// The async version of <see cref="IDataBaseSession.GetTableMetaData{T}"/>.
        /// </summary>
        Task<TableMetaData> GetTableMetaDataAsync(string tableName);
    }
}