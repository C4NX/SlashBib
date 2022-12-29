using DSharpPlus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    internal static class ValuesFormatter
    {
        public static string Format(string value, Dictionary<string, object>? values, object[] args, ReadablitySettings? readablitySettings = null)
        {
            if(values != null)
            {
                return Regex.Replace(value, @"{([^}]*)}", match => 
                {
                    string matchValue = match.Value;
                    string[] keyNameAndFilters = matchValue
                        .TrimStart('{')
                        .TrimEnd('}')
                        .Split('|', StringSplitOptions.RemoveEmptyEntries);

                    string keyName = keyNameAndFilters[0];
                    string? transformedValue = null;

                    // check for a param
                    if (keyName.StartsWith(':'))
                    {
                        // check if it's a number and in bound of args
                        if (int.TryParse(keyName[1..], out int idx) && idx >= 0 && idx < args.Length)
                        {
                            transformedValue = ReadableToString(args[idx], readablitySettings) ?? matchValue;
                        }
                    }
                    // check in values
                    else if (values.ContainsKey(keyName))
                    {
                        transformedValue = ReadableToString(values[keyName], readablitySettings) ?? matchValue;
                    }

                    if (transformedValue is not null)
                    {
                        // apply filters
                        if (keyNameAndFilters.Length > 1)
                        {
                            for (int i = 1; i < keyNameAndFilters.Length; i++)
                            {
                                transformedValue = Filter(transformedValue, keyNameAndFilters[i], readablitySettings);
                            }
                        }

                        return transformedValue;
                    }
                    return matchValue;
                });
            }
            return value;
        }

        public static string Filter(string value, string filter, ReadablitySettings? readablitySettings)
        {
            string filterName = filter.ToLowerInvariant();

            // switch for basic filters
            switch (filter.ToLowerInvariant())
            {
                case "bold":
                    return Formatter.Bold(value);
                case "Italic":
                    return Formatter.Italic(value);
                case "code":
                    return Formatter.InlineCode(value);
                case "json":
                    return Formatter.BlockCode(value, "json");
                case "spoiler":
                    return Formatter.Spoiler(value);
                case "upper":
                    return value.ToUpper();
                case "lower":
                    return value.ToLower();
                case "dump":
                    Console.WriteLine($"Dump: {value}");
                    return value;
                default:
                    // try with custom filters
                    if (readablitySettings is not null)
                    {
                        foreach (var filterFactory in readablitySettings.CustomFilters)
                        {
                            // try to find it
                            if (filterFactory.Key.ToLowerInvariant() == filterName)
                            {
                                // run it and close the foreach after
                                return filterFactory.Value(filterName);
                            }
                        }
                    }
                    return value;
            }
        }
        public static string? ReadableToString(object obj, ReadablitySettings? readablitySettings)
        {
            ReadablitySettings settings = readablitySettings ?? ReadablitySettings.Default;

            // JToken Support
            if (obj is JToken jsonObject)
            {
                string jsonString = jsonObject.ToString();
                return settings.DiscordReady ? Formatter.BlockCode(jsonString, "json") : jsonString;
            }
            // enumerable support
            else if (obj is not string && typeof(IEnumerable<object>).IsAssignableFrom(obj.GetType()))
            {
                return string.Join(',', Enumerable.ToArray((IEnumerable<object>)obj));
            }
            else
            {
                return obj.ToString();
            }
        }

        public static string? WithLength(this string? value, int maxLength)
            => value?.Substring(0, Math.Min(value.Length, maxLength));
    }

    public class ReadablitySettings
    {
        private static readonly ReadablitySettings _default = new ReadablitySettings();

        public static ReadablitySettings Default
            => _default;

        public bool DiscordReady { get; set; }

        public Dictionary<string, Func<string, string>> CustomFilters { get; set; }
            = new Dictionary<string, Func<string, string>>();

        public ReadablitySettings AddCustomFilter(string filter, Func<string, string> func)
        {
            CustomFilters.Add(filter, func);
            return this;
        }
    }
}
