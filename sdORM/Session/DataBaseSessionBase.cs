using System.Data.Common;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Helper;
using sdORM.Mapping;
using sdORM.Session.Interfaces;

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

        public virtual void Connect()
        {
            this.Connection.Open();
        }

        public virtual async Task ConnectAsync()
        {
            await this.Connection.OpenAsync();
        }

        public void AddTransaction()
        {
            this.Transaction = this.Connection.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            if(this.Transaction != null)
            {
                this.Transaction.Rollback();
                this.Transaction = null;
            }
        }
        
        protected DbCommand GenerateCommand(ParameterizedSql parameterizedSql)
        {
            Guard.NotNull(parameterizedSql, nameof(parameterizedSql));

            var command = this.Connection.CreateCommand();

            command.CommandText = parameterizedSql;

            if (this.Transaction != null)
                command.Transaction = this.Transaction;
            
            foreach (var currentParameter in parameterizedSql.Parameters.EmptyIfNull())
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