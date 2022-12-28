using SlashBib.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SlashBib.Core.Utilities;

public static class Translator
{
    /// <summary>
    /// Translate a key with a lang or a default languages.
    /// </summary>
    /// <param name="key">The key of this lang</param>
    /// <param name="lang">the lang name or null</param>
    /// <param name="guildId">TODO</param>
    /// <returns>The translated string</returns>
    public static string Translate(string? key, string? lang = null, bool dynamicTranslation = true)
    {
        if (key is null)
            return "<null>";

        string value = string.Empty;
        SlashBibBot instance = SlashBibBot.GetInstance();
        SlashConfiguration configuration = instance.Configuration;
        if (lang is null)
        {
            string keyPath = $"{configuration.GetDefaultLanguage()}_{key}";
            value = configuration.GetOption<string>(keyPath) ?? keyPath;
        }
        else
            value = configuration.GetOption<string>($"{lang}_{key}") ?? $"{key} (as {lang})";

        return dynamicTranslation ? instance.Strings.ToString(value) : value;
    }

    /// <summary>
    /// Get an <see cref="IEnumerable{T}"/> of the available languages.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetAvailablesLanguages()
        => SlashBibBot.GetInstance().Configuration.LanguagesNames ?? Enumerable.Empty<string>();
}