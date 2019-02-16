using System.Collections.Generic;

namespace sdORM.Common.SqlSpecifics
{
    public class ParameterizedSql
    {
        public string Sql { get; set; }

        public IList<SqlParameter> Parameters { get; set; }

        public ParameterizedSql()
        {
            this.Parameters = new List<SqlParameter>();
        }

        public static implicit operator string(ParameterizedSql parameterizedSql)
        {
            return parameterizedSql.Sql;
        }

        public static implicit operator ParameterizedSql(string sql)
        {
            return new ParameterizedSql
            {
                Sql = sql
            };
        }
    }
}