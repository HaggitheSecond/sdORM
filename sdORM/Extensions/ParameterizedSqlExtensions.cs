using System.Linq;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Extensions
{
    public static class ParameterizedSqlExtensions 
    {
        public static string GetWithReplacedParameters(this ParameterizedSql self)
        {
            return self.Parameters.Aggregate(self.Sql, 
                (current, currentParameter) => current.Replace(currentParameter.ParameterName,
                    currentParameter.Value is string ? $"\"{currentParameter.Value}\"" : currentParameter.Value.ToString()));
        }
    }
}