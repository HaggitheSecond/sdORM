using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using sdORM.Common.SqlSpecifics;
using sdORM.Entities;

namespace sdORM
{
    public interface IDataBaseSession : IDisposable
    {
        void Connect();

        IList<T> Query<T>() where T : new();
        IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : new();
        IList<T> Query<T>(ParameterizedSql parameterizedSql) where T : new();

        T GetByID<T>(object id) where T : new();

        T SaveOrUpdate<T>(T entity);

        T Save<T>(T entity);

        T Update<T>(T entity);

        TableMetaData GetTableMetaData(string tableName);
    }
}