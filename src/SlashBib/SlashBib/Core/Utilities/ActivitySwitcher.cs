using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace SlashBib.Core.Utilities
{
    public class ActivitySwitcher
    {
        private IList<DiscordActivity> _activities;
        private readonly SlashBibBot _instance;

        public int Count
            => _activities.Count;

        public ActivitySwitcher(SlashBibBot slashBibBot, IEnumerable<DiscordActivity>? activities = null)
        {
            _instance = slashBibBot;
            _activities = activities == null 
                ? new List<DiscordActivity>()
                : new List<DiscordActivity>(activities);
        }

        public async Task Switch(Random? random = null)
        {
            if (_activities.Count > 0)
            {
                var rowActivity = _activities[(random ?? new Random()).Next(0, _activities.Count)];
                var applyActivity = new DiscordActivity(_instance.Strings.ToString(rowActivity.Name), rowActivity.ActivityType)
                {
                    StreamUrl = rowActivity.StreamUrl
                };

                await _instance.Discord.UpdateStatusAsync(applyActivity);
            }
        }

        public class DynamicValue
        {
            private readonly Func<object> _valueFactory;

            public DynamicValue(Func<object> factory)
            {
                _valueFactory = factory;
            }

            public override string? ToString()
                => _valueFactory().ToString();
        }
    }
}
