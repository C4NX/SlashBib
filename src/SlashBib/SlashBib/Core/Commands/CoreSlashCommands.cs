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
                .WithDescription("Made with love by NX 💜")
                .AddField("💻 System"
                    , $"Runtime: {Formatter.InlineCode(RuntimeInformation.Version)}\nOS: {Formatter.InlineCode(System.Runtime.InteropServices.RuntimeInformation.OSDescription)}\nPing: {Formatter.InlineCode(ctx.Client.Ping.ToString())}")
                .AddField("🖼 Discord",
                    $"Guilds: {Formatter.InlineCode(ctx.Client.Guilds.Count.ToString())}\nActivity: {Formatter.InlineCode(instance.Activity.Count.ToString())}\r\nCommands: {Formatter.InlineCode(GetCommandsCount(ctx.SlashCommandsExtension).ToString())}"), true);
    }

    [SlashCommand("reload", "Owner: reload me !")]
    [SlashRequireOwner]
    public async Task OwnerReload(InteractionContext ctx)
    {
        await SlashBibBot.GetInstance()
            .ReloadAsync();
        await ctx.CreateResponseAsync("Ok.", true);
    }

    private static long GetCommandsCount(SlashCommandsExtension? ext)
    {
        if(ext == null)
            return 0;

        return ext.RegisteredCommands.SelectMany(x => x.Value)
            .LongCount();
    }
}