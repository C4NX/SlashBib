using SlashBib.Core.Configuration;

namespace SlashBib.Core.Utilities;

public static class CommandHelper
{
    public static string Translate(string key, string? lang = null, ulong? guildId = null)
    {
        var cfg = SlashBibBot.GetInstance().Configuration;

        return lang switch
        {
            null => cfg.GetOption<string>(key) ?? key,
            SlashConfiguration.LangSelectDefaultName => throw new NotImplementedException("Implements of Mongodb first"),
            _ => cfg.GetOption<string>($"{lang}_{key}") ?? $"{key} (as {lang})",
        };
    }
}