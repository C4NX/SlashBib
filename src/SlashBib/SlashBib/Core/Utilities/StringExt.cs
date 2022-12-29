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
    internal static class StringExt
    {
        public static string NamedFormat(this string value, Dictionary<string, object>? values, ReadablitySettings? readablitySettings = null)
        {
            if(values != null)
            {
                return Regex.Replace(value, @"{([^}]*)}", match => 
                {
                    var r = match.Value;
                    var keyNameAndFilters = r.TrimStart('{').TrimEnd('}').Split('|', StringSplitOptions.RemoveEmptyEntries);

                    // return if the key is contains
                    if (values.ContainsKey(keyNameAndFilters[0]))
                    {
                        string replacedValue = ReadableToString(values[keyNameAndFilters[0]], readablitySettings) ?? r;

                        // apply filters
                        if (keyNameAndFilters.Length > 1)
                        {
                            for (int i = 1; i < keyNameAndFilters.Length; i++)
                            {
                                string filterName = keyNameAndFilters[i].ToLowerInvariant();

                                // switch for basic filters
                                switch (filterName)
                                {
                                    case "bold":
                                        replacedValue = Formatter.Bold(replacedValue);
                                        break;
                                    case "Italic":
                                        replacedValue = Formatter.Italic(replacedValue);
                                        break;
                                    case "code":
                                        replacedValue = Formatter.InlineCode(replacedValue);
                                        break;
                                    case "json":
                                        replacedValue = Formatter.BlockCode(replacedValue, "json");
                                        break;
                                    case "spoiler":
                                        replacedValue = Formatter.Spoiler(replacedValue);
                                        break;
                                    case "upper":
                                        replacedValue = replacedValue.ToUpper();
                                        break;
                                    case "lower":
                                        replacedValue = replacedValue.ToLower();
                                        break;
                                    case "dump":
                                        Console.WriteLine($"Dump: {replacedValue}");
                                        break;
                                    default:

                                        // try with custom filters
                                        if(readablitySettings is not null)
                                        {
                                            foreach (var filterFactory in readablitySettings.CustomFilters)
                                            {
                                                // try to find it
                                                if(filterFactory.Key.ToLowerInvariant() == filterName)
                                                {
                                                    // run it and close the foreach after
                                                    replacedValue = filterFactory.Value(replacedValue);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }

                        // return the transformed (or not) value
                        return replacedValue;
                    }
                    return r;
                });
            }
            return value;


            /*return values == null
                ? value
                : values.Aggregate(value, (current, parameter) =>
                    {
                        // filter processing
                        if (current.Contains('|'))
                        {
                            if (current.Contains("{ " + parameter.Key + "|bold}"))
                            {
                                var spl = current.Split('|', 2, StringSplitOptions.RemoveEmptyEntries);
                                return current.Replace("{" + parameter.Key + "}", ReadableToString(parameter.Value, readablitySettings));
                            }
                        }
                        // normal processing
                        else if (current.Contains("{" + parameter.Key + "}"))
                            return current.Replace("{" + parameter.Key + "}", ReadableToString(parameter.Value, readablitySettings));

                        return current;
                    });
            */
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
