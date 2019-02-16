using System;
using System.Collections.Generic;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Session
{
    public interface IRawDatabaseSession : IDisposable
    {
        void Connect();

        IList<Dictionary<string, object>> ExecuteReader(ParameterizedSql sql);

        IList<T> ExecuteReader<T>(ParameterizedSql sql) where T : new();

        int ExecuteNonQuery(ParameterizedSql sql);
    }
}