using System.Data;
using System.Data.Common;
using sdORM.Common;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Helper;
using sdORM.Mapping;

namespace sdORM.Session
{
    public abstract class DatabaseSessionBase
    {
        protected DbConnection Connection { get; }
        protected ISqlSpecificProvider SqlSpecificProvider { get; }
        protected EntityMappingProvider EntityMappingProvider { get; }

        protected DbTransaction Transaction { get; private set; }

        protected DatabaseSessionBase(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecificProvider sqlSpecificProvider)
        {
            Guard.NotNull(connection, nameof(connection));
            Guard.NotNull(entityMappingProvider, nameof(entityMappingProvider));
            Guard.NotNull(sqlSpecificProvider, nameof(sqlSpecificProvider));

            this.Connection = connection;
            this.SqlSpecificProvider = sqlSpecificProvider;
            this.EntityMappingProvider = entityMappingProvider;
        }

        public void AddTransaction()
        {
            this.Transaction = this.Connection.BeginTransaction();
        }

        public DbCommand GenerateCommand(string sql)
        {
            return this.GenerateCommand(new ParameterizedSql
            {
                Sql = sql
            });
        }

        public DbCommand GenerateCommand(ParameterizedSql sql)
        {
            var command = this.Connection.CreateCommand();

            command.CommandText = sql.Sql;

            if (this.Transaction != null)
                command.Transaction = this.Transaction;
            
            foreach (var currentParameter in sql.Parameters.EmptyIfNull())
            {
                var parameter = command.CreateParameter();

                parameter.ParameterName = currentParameter.ParameterName;
                parameter.Value = currentParameter.Value;

                command.Parameters.Add(parameter);
            }

            return command;
        }

        public void Dispose()
        {
            this.Transaction?.Commit();
            this.Connection.Close();

            this.Transaction?.Dispose();
            this.Connection.Dispose();
        }
    }
}