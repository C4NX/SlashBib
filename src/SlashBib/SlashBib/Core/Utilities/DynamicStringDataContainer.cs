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

        public DynamicStringDataContainer() { 
            _dynData = new Dictionary<string, object>();
        }

        public object? this[string key]
        {
            get
                => _dynData.ContainsKey(key) ? _dynData[key] : null;
            set
                => _dynData[key] = value ?? string.Empty;
        }

        public string ToString(string value)
            => value.NamedFormat(_dynData);

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

            /// <summary>
            /// Create a new dynamic value from a factory.
            /// </summary>
            /// <param name="factory">The factory to use</param>
            public DynamicValue(Func<object> factory)
            {
                _valueFactory = factory;
            }

            /// <summary>
            /// Get the generated value from the factory.
            /// </summary>
            /// <returns>the value generated</returns>
            public override string? ToString()
                => _valueFactory().ToString();
        }
    }
}
