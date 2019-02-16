using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Mapping;
using sdORM.Session.Interfaces;

namespace sdORM.Session.Implementations
{
    public class RawDatabaseSessionAsync : DatabaseSessionBase, IRawDatabaseSessionAsync
    {
        public RawDatabaseSessionAsync(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecificProvider sqlSpecificProvider)
            : base(connection, entityMappingProvider, sqlSpecificProvider)
        {
        }
        
        public async Task<IList<Dictionary<string, object>>> ExecuteReaderAsync(ParameterizedSql sql)
        {
            using (var command = this.GenerateCommand(sql))
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return default(IList<Dictionary<string, object>>);

                var results = new List<Dictionary<string, object>>();
                var columns = reader.GetColumns().ToList();

                while (await reader.ReadAsync())
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

        public async Task<IList<T>> ExecuteReaderAsync<T>(ParameterizedSql sql) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.GenerateCommand(sql))
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return default(IList<T>);

                var results = new List<T>();

                while (await reader.ReadAsync())
                {
                    results.Add(reader.LoadWithEntityMapping(mapping));
                }

                return results;
            }
        }

        public Task<int> ExecuteNonQueryAsync(ParameterizedSql sql)
        {
            using (var command = this.GenerateCommand(sql))
            {
                return command.ExecuteNonQueryAsync();
            }
        }
    }
}