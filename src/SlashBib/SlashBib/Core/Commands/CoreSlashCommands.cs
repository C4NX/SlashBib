using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Emzi0767.Utilities;
using SlashBib.Core.Utilities;

namespace SlashBib.Core.Commands;

public class CoreSlashCommands : ApplicationCommandModule
{
    [SlashCommand("info", "Show some information about SlashBib.")]
    [DescriptionLocalization(Localization.French, "Affiche des informations sur SlashBib.")]
    public async Task SlashInformation(InteractionContext ctx)
    {
        var instance = SlashBibBot.GetInstance();

        await ctx.CreateResponseAsync(
            instance.GetEmbedBuilder()
                .WithTitle("📃 Information")
                .WithDescription("Made with love by NX#2747 💜")
                .WithThumbnail(ctx.Client.CurrentUser.AvatarUrl)
                .AddField("💻 System"
                    , instance.Strings.ToString("Runtime: {runtime.dotnet|code}\nOS: {runtime.os|code}\nPing: {bot.ping|code}ms"))
                .AddField("🖼 Discord",
                    instance.Strings.ToString("Guilds: {bot.guild.count|code}\nActivity: {bot.activity.count|code}\r\nCommands: " + $"{Formatter.InlineCode(GetCommandsCount(ctx.SlashCommandsExtension).ToString())}"))
                .AddField("🚗 Engine",
                    instance.Strings.ToString("Strings: {bot.strings.count|code}\nLanguages: {bot.langs|code|upper}")), true);
    }

    [SlashCommand("reload", "Reload all the configuration (Owner only)")]
    [SlashRequireOwner]
    public async Task OwnerReload(InteractionContext ctx)
    {
        await SlashBibBot.GetInstance()
            .ReloadAsync();
        await ctx.CreateResponseAsync(Translator.Translate("reload"), true);
    }

    private static long GetCommandsCount(SlashCommandsExtension? ext)
    {
        if(ext == null)
            return 0;

        return ext.RegisteredCommands.SelectMany(x => x.Value)
            .LongCount();
    }
}