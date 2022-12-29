using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Emzi0767.Utilities;
using SixLabors.ImageSharp.Formats.Webp;
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
            instance.GetEmbed()
                .AddField("💻 System", "info_1", true, false)
                .AddField("🖼 Discord", "info_2", true, false)
                .AddField("🚗 Engine", "info_3", true, false)
                .WithDescription("love", false)
                .Builder
                    .WithTitle("📃 Information")
                    .WithThumbnail(ctx.Client.CurrentUser.AvatarUrl), true);
    }

    [SlashCommand("avatar", "Show avatar")]
    [DescriptionLocalization(Localization.French, "Affiche un avatar.")]
    public async Task Avatar(InteractionContext ctx, [Option("user", "What user ?")] DiscordUser user)
    {
        var instance = SlashBibBot.GetInstance();

        await ctx.CreateResponseAsync(
            instance.GetEmbed()
                .WithDescription("avatar", false, user)
                .Builder
                    .WithTitle($"{user.Username}'s Avatar")
                    .WithImageUrl(user.GetAvatarUrl(ImageFormat.Png)), true);
    }

    [SlashCommand("test", "TestRender")]
    public async Task Test(InteractionContext ctx)
    {
        var instance = SlashBibBot.GetInstance();

        var img = new ImageRenderer(false);

        var inte = new DiscordInteractionResponseBuilder();
        inte.AddEmbed(instance.GetEmbed()
                .WithDescription("test", false)
                .Builder);

        inte.AddFile("image.webp", await img.RenderStreamAsync(new SixLabors.ImageSharp.Size(100, 100), new WebpEncoder()));

        await ctx.CreateResponseAsync(inte);
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