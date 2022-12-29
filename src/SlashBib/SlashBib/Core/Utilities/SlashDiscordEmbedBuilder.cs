using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    public class SlashDiscordEmbedBuilder
    {
        private DiscordEmbedBuilder _builder;
        private SlashBibBot _slashBib;

        public DiscordEmbedBuilder Builder
            => _builder;

        public SlashDiscordEmbedBuilder()
        {
            _slashBib = SlashBibBot.GetInstance();
            _builder = new DiscordEmbedBuilder();
        }

        public SlashDiscordEmbedBuilder(DiscordEmbed embed)
        {
            _builder = new DiscordEmbedBuilder(embed);
            _slashBib = SlashBibBot.GetInstance();
        }

        public SlashDiscordEmbedBuilder AddField(string name,string valueKey, bool useGlobal, bool inline, params object[] args)
        {
            _builder.AddField(name, Translator.RawTranslate(valueKey, null, !useGlobal, true, args), inline);
            return this;
        }

        public SlashDiscordEmbedBuilder WithDescription(string key, bool useGlobal, params object[] args)
        {
            _builder.Description = Translator.RawTranslate(key, null, !useGlobal, true, args);
            return this;
        }
    }
}
