using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;

namespace sdORM
{
    public interface IDataBaseSessionAsync : IDisposable
    {
        Task ConnectAsync();

        Task<IList<T>> QueryAsync<T>() where T : new();
        Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : new();
        Task<IList<T>> QueryAsync<T>(ParameterizedSql sql) where T : new();

        Task<T> GetByIDAsync<T>(object id) where T : new();

        Task<T> SaveOrUpdateAsync<T>(T entity);

        Task<T> SaveAsync<T>(T entity);

        Task<T> UpdateAsync<T>(T entity);

        Task<TableMetaData> GetTableMetaDataAsync(string tableName);
    }
}