using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Session
{
    public interface IRawDatabaseSessionAsync : IDisposable
    {
        Task ConnectAsync();

        Task<IList<Dictionary<string, object>>> ExecuteReaderAsync(ParameterizedSql sql);

        Task<IList<T>> ExecuteReaderAsync<T>(ParameterizedSql sql) where T : new();

        Task<int> ExecuteNonQueryAsync(ParameterizedSql sql);
    }
}