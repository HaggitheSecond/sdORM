using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using MySql.Data.MySqlClient;
using sdORM.Common;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;
using sdORM.Session;

namespace sdORM.MySql.Session
{

    public class MySqlDataBaseSession : DataBaseSession
    {
        private readonly MySqlSqlProvider _sqlProvider;

        public MySqlDataBaseSession(SdOrmConfig config)
            : base(new MySqlConnection(config.ConnectionString), config.Mappings)
        {
            this._sqlProvider = new MySqlSqlProvider();
        }

        protected override ParameterizedSql GetSqlForPredicate<T>(Expression<Func<T, bool>> predicate, EntityMapping<T> mapping)
        {
            return this._sqlProvider.GetSqlForPredicate(predicate, mapping, new ExpressionToMySqlProvider());
        }

        protected override ParameterizedSql GetSqlForGetById<T>(object id, EntityMapping<T> mapping)
        {
            return this._sqlProvider.GetSqlForGetById(id, mapping);
        }

        protected override void SetIdAfterSave<T>(T entity, IDbCommand command, EntityMapping<T> mapping)
        {
            this._sqlProvider.SetIdAfterSave(entity, command, mapping);
        }

        protected override ParameterizedSql GetSqlForSave<T>(T entity, EntityMapping<T> mapping)
        {
            return this._sqlProvider.GetSqlForSave(entity, mapping);
        }

        protected override ParameterizedSql GetSqlForUpdate<T>(T entity, EntityMapping<T> mapping)
        {
            return this._sqlProvider.GetSqlForUpdate(entity, mapping);
        }

        protected override string GetSqlForTableMetaData(string tableName)
        {
            return this._sqlProvider.GetSqlForTableMetaData(tableName);
        }

        protected override string GetSqlForCheckIfTableExtists(string tableName)
        {
            return this._sqlProvider.GetSqlForCheckIfTableExtists(tableName);
        }
        
        protected override StringBuilder GetSelectStatementForMapping<T>(EntityMapping<T> mapping)
        {
            return this._sqlProvider.GetSelectStatementForMapping(mapping);
        }

        protected override IEnumerable<SqlParameter> GetParametersForMapping<T>(T entity, EntityMapping<T> mapping)
        {
            return this._sqlProvider.GetParametersForMapping(entity, mapping);
        }

        protected override DbCommand GenerateIDBCommand(string sql)
        {
            var cmd = this._connection.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }
    }
}