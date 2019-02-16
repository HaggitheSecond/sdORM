using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Mapping;

namespace sdORM.Session
{
    public class RawDatabaseSession : DatabaseSessionBase, IRawDatabaseSession
    {
        public RawDatabaseSession(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecificProvider sqlSpecificProvider)
            : base(connection, entityMappingProvider, sqlSpecificProvider)
        {
        }

        public IList<Dictionary<string, object>> ExecuteReader(ParameterizedSql sql)
        {
            using (var command = this.GenerateCommand(sql))
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return default(IList<Dictionary<string, object>>);

                var results = new List<Dictionary<string, object>>();
                var columns = reader.GetColumns().ToList();

                while (reader.Read())
                {
                    var item = new Dictionary<string, object>();

                    foreach (var (ordinal, columnName) in columns)
                    {
                        item.Add(columnName, reader.GetValue(ordinal));
                    }

                    results.Add(item);
                }

                return results;
            }
        }

        public IList<T> ExecuteReader<T>(ParameterizedSql sql) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.GenerateCommand(sql))
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return default(IList<T>);

                var results = new List<T>();

                while (reader.Read())
                {
                    results.Add(reader.LoadWithEntityMapping(mapping));
                }

                return results;
            }
        }

        public int ExecuteNonQuery(ParameterizedSql sql)
        {
            using (var command = this.GenerateCommand(sql))
            {
                return command.ExecuteNonQuery();
            }
        }
    }
}