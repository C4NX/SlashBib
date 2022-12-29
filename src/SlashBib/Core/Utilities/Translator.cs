using SlashBib.Core.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Remoting;

namespace SlashBib.Core.Utilities;

/// <summary>
/// A set of utils to translate strings with languages for <see cref="SlashBibBot"/>.
/// </summary>
public static class Translator
{
    /// <summary>
    /// RawTranslate a string with the global translation prefix '_'
    /// </summary>
    /// <param name="key">the key to translate</param>
    /// <returns>The translated value</returns>
    public static string GTranslate(string key, params object[] args)
        => RawTranslate(key, null, false, true, args);

    /// <summary>
    /// RawTranslate a string with the default language only.
    /// </summary>
    /// <param name="key">the key to translate</param>
    /// <returns>The translated value</returns>
    public static string DTranslate(string key, params object[] args)
        => RawTranslate(key, null, true, true, args);

    public static string Translate(string key, params object[] args)
    {
        //TODO: RawTranslate with a user selected language.
        return RawTranslate(key, null, true, true, args);
    }

    /// <summary>
    /// RawTranslate a string
    /// </summary>
    /// <param name="key">The key to translate</param>
    /// <param name="lang">The lang to use, or null for default</param>
    /// <param name="useLang">false to use the global prefix '_', true to use the lang or default lang</param>
    /// <param name="dynamicTranslation">If the string is going to pass the <see cref="DynamicStringDataContainer"/> of <see cref="SlashBibBot"/></param>
    /// <returns>The translated value</returns>
    public static string RawTranslate(string key, string? lang, bool useLang, bool dynamicTranslation, object[] args)
    {
        SlashBibBot instance = SlashBibBot.GetInstance();
        SlashConfiguration configuration = instance.Configuration;
        string value;

        if (!useLang)
        {
            value = configuration.GetOption<string>($"_{key}") ?? $"_{key}";
        }
        else if (lang is null)
        {
            string keyPath = $"{configuration.GetDefaultLanguage()}_{key}";
            value = configuration.GetOption<string>(keyPath) ?? keyPath;
        }
        else
            value = configuration.GetOption<string>($"{lang}_{key}") ?? $"{key} (as {lang})";

        // last pass on a string format for translation
        return (dynamicTranslation ? instance.Strings.ToString(value, args: args) : value);
    }

    /// <summary>
    /// Get an <see cref="IEnumerable{T}"/> of the available languages.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetAvailablesLanguages()
        => SlashBibBot.GetInstance().Configuration.LanguagesNames ?? Enumerable.Empty<string>();
}