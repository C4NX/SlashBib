using DSharpPlus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    public class DynamicStringDataContainer : IEnumerable<string>
    {
        private readonly Dictionary<string, object> _dynData;
        private readonly ILogger _logger;

        public int Count
            => _dynData.Count;

        public ReadablitySettings ReadablitySettings { get; set; }

        public DynamicStringDataContainer() { 
            _dynData = new Dictionary<string, object>();
            _logger = Log.ForContext<DynamicStringDataContainer>();
            ReadablitySettings = new ReadablitySettings();
        }

        public object? this[string key]
        {
            get
                => _dynData.ContainsKey(key) ? _dynData[key] : null;
            set
            {
                _dynData[key] = value ?? string.Empty;
                _logger.Debug("Setting up string {key} with a {type} : {value}", key, value?.GetType().Name, (value is string) ? value : "<obj>");
            }
        }

        public string ToString(string value, ReadablitySettings? readablitySettings = null)
        {
            return value.NamedFormat(_dynData, readablitySettings ?? ReadablitySettings);
        }

        public IEnumerator<string> GetEnumerator()
            => _dynData.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// A simple factory class for <see cref="DynamicStringDataContainer"/>.
        /// </summary>
        public class DynamicValue
        {
            private readonly Func<object> _valueFactory;
            private readonly ReadablitySettings? _readablitySettings;

            /// <summary>
            /// Create a new dynamic value from a factory.
            /// </summary>
            /// <param name="factory">The factory to use</param>
            public DynamicValue(Func<object> factory, ReadablitySettings? readablitySettings = null)
            {
                _valueFactory = factory;
                _readablitySettings = readablitySettings;
            }

            /// <summary>
            /// Get the generated value from the factory.
            /// </summary>
            /// <returns>the value generated</returns>
            public override string? ToString()
                => StringExt.ReadableToString(_valueFactory(), _readablitySettings);
        }
    }
}
