using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Common.SqlSpecifics;
using sdORM.Entities;

namespace sdORM.Interfaces
{
    public interface IDataBaseSession : IDisposable
    {
        EntityMappingProvider EntityMappingProvider { get; }

        bool IsConnected { get; }

        void Connect();
        Task ConnectAsync();

        IList<T> Query<T>() where T : new();
        Task<IList<T>> QueryAsync<T>() where T : new();

        IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : new();
        Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : new();

        IList<T> Query<T>(ParameterizedSql query) where T : new();
        Task<IList<T>> QueryAsync<T>(ParameterizedSql sql) where T : new();

        T GetByID<T>(object id) where T : new();
        Task<T> GetByIDAsync<T>(object id) where T : new();

        T SaveOrUpdate<T>(T entity);
        Task<T> SaveOrUpdateAsync<T>(T entity);

        T Save<T>(T entity);
        Task<T> SaveAsync<T>(T entity);

        T Update<T>(T entity);
        Task<T> UpdateAsync<T>(T entity);

        TableMetaData GetTableMetaData(string tableName);
        Task<TableMetaData> GetTableMetaDataAsync(string tableName);
    }
}