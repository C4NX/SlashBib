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
        private readonly Dictionary<string, object> _dynData;
        private readonly SlashBibBot _instance;

        public int Count
            => _activities.Count;

        public ActivitySwitcher(SlashBibBot slashBibBot, IEnumerable<DiscordActivity>? activities = null)
        {
            _instance = slashBibBot;
            _activities = activities == null 
                ? new List<DiscordActivity>()
                : new List<DiscordActivity>(activities);
            _dynData = new Dictionary<string, object>();
        }

        public object? this[string key]
        {
            get 
                => _dynData.ContainsKey(key) ? _dynData[key] : null;
            set
                => _dynData[key] = value
                                   ?? string.Empty;
        }

        public async Task Switch(Random? random = null)
        {
            if (_activities.Count > 0)
            {
                var rowActivity = _activities[(random ?? new Random()).Next(0, _activities.Count)];
                var applyActivity = new DiscordActivity(rowActivity.Name.NamedFormat(_dynData), rowActivity.ActivityType)
                {
                    StreamUrl = rowActivity.StreamUrl
                };

                await _instance.Discord.UpdateStatusAsync(applyActivity);
            }
        }
    }
}
