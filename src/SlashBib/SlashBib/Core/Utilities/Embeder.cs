using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    public static class Embeder
    {
        public static DiscordEmbedBuilder CreateError(SlashBibBot slashBibBot, Exception exception, DiscordUser? discordUser)
        {
            bool isOwner = discordUser == null 
                ? false 
                : slashBibBot.Discord.CurrentApplication.Owners.Any(x=>discordUser.Id == x.Id);
            string errorMessage = exception.Message;

            var embedBuilder = slashBibBot.GetEmbed()
                            .Builder
                            .WithTitle("An error has occurred.")
                            .WithDescription("We are sorry for this inconvenience, it will not happen again.")
                            .AddField("Message", Formatter.BlockCode(errorMessage.WithLength(1000)))
                            .WithColor(DiscordColor.Red);

            if (discordUser != null && isOwner)
            {
                var innerExceptionString = exception.InnerException != null ? "\n" + string.Join("\n", exception.GetInnerExceptions().Select(x => $"- {x.GetType().Name}")) : null;
                embedBuilder.AddField("Type", Formatter.BlockCode((exception.GetType().Name + $"{(innerExceptionString == null ? string.Empty : innerExceptionString)}").WithLength(1000), "markdown"))
                            .AddField("Stack", Formatter.BlockCode((string.IsNullOrEmpty(exception.StackTrace) ? Environment.StackTrace : exception.StackTrace).WithLength(1000), "markdown"));

                if (slashBibBot.IsDebug)
                    embedBuilder.AddField("Thread", Formatter.BlockCode($"{Thread.CurrentThread.ManagedThreadId} ({Thread.CurrentThread.Name ?? "Unnamed"})"));
            }

            return embedBuilder;
        }
    }
}
