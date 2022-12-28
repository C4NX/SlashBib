using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    internal static class StringExt
    {
        public static string NamedFormat(this string value, Dictionary<string, object>? values)
        {
            return values == null
                ? value
                : values.Aggregate(value,
                    (current, parameter) => current.Replace("{" + parameter.Key + "}", parameter.Value.ToString()));
        }

        public static string? WithLength(this string? value, int maxLength)
            => value?.Substring(0, Math.Min(value.Length, maxLength));
    }
}
