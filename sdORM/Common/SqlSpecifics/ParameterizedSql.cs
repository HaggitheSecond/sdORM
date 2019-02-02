using System.Collections.Generic;

namespace sdORM.Common.SqlSpecifics
{
    public class ParameterizedSql
    {
        public string Sql { get; set; }

        public IList<SqlParameter> Parameters { get; set; }
    }
}