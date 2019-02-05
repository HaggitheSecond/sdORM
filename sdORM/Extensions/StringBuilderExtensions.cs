using System.Collections.Generic;
using System.Text;

namespace sdORM.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendJoin(this StringBuilder self, string seperator, IEnumerable<string> parts)
        {
            return self.Append(string.Join(seperator, parts));
        }
    }
}