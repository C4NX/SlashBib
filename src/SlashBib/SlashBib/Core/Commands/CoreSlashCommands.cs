using DSharpPlus.SlashCommands;

namespace SlashBib.Core.Commands;

public class CoreSlashCommands : ApplicationCommandModule
{
    [SlashCommand("info", "Show some information about SlashBib.")]
    public async Task CmdInformation(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(
            SlashBibBot.GetInstance().GetEmbedBuilder()
                .WithTitle("Information")
                .WithDescription("TODO: ..."), true);
    }
}