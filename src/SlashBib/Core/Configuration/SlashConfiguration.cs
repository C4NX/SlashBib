using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlashBib.Core.Configuration.Models;

namespace SlashBib.Core.Configuration;

public class SlashConfiguration
{
    [JsonIgnore] private JsonSerializer? _serializer;
    [JsonIgnore] private string? _filename;
    [JsonIgnore] private string _cache_default_lang = "en";

    private Dictionary<string, string?>? _secretData;
    
    [JsonProperty("options")] private readonly Dictionary<string, JToken?>? _options;
        
    [JsonProperty("secret")]
    public string SecretFilename { get; private set; }
        = ".secret.json";
    
    [JsonProperty("debug")] public bool Debug { get; set; }
    [JsonProperty("langs")] public IList<string>? LanguagesNames { get; set; }
    [JsonConverter(typeof(DiscordEmbedConverter))] [JsonProperty("embed")] public DiscordEmbed? DefaultEmbed { get; set; }
    
    private SlashConfiguration()
    {
        _secretData = new Dictionary<string, string?>();
        _options = new Dictionary<string, JToken?>();
    }

    private SlashConfiguration LoadFile(string filename)
    {
        _serializer ??= JsonSerializer.CreateDefault();
        _filename = filename;
        LanguagesNames?.Clear();

        using (FileStream fs = File.OpenRead(filename))
        {
            using (StreamReader reader = new StreamReader(fs))
            {
                _serializer.Populate(reader, this);

                // load the first language name in the _cache_default_lang
                if (LanguagesNames?.Count > 0)
                    _cache_default_lang = LanguagesNames[0];
            }
        }

        if (File.Exists(SecretFilename))
        {
            using (FileStream fsSecret = File.OpenRead(SecretFilename))
            {
                using (StreamReader reader = new StreamReader(fsSecret))
                {
                    using (JsonReader jsonReader = new JsonTextReader(reader))
                    {
                        _secretData = _serializer.Deserialize<Dictionary<string, string?>>(jsonReader);
                    }
                }
            }
        }
        
        return this;
    }

    public void Reload()
    {
        if (_filename == null)
            throw new InvalidOperationException("Can't reload when no filename is stored");
        LoadFile(_filename);
    }

    public string? GetSecret(string key, string? defaultValue)
    {
        if (_secretData != null && _secretData.ContainsKey(key))
            return _secretData[key];
        return defaultValue;
    }

    public T? GetOption<T>(string key)
    {
        if (_options == null
            || !_options.ContainsKey(key))
            return default;

        var val = _options[key];
        return val == null ? default : val.Value<T>();
    }
    
    public static SlashConfiguration FromFile(string filename)
    {
        return new SlashConfiguration()
            .LoadFile(filename);
    }

    public IEnumerable<string> GetOptionsNames()
        => _options?.Keys ?? Enumerable.Empty<string>();

    /// <summary>
    /// Get the default language name, by default it's 'en'
    /// </summary>
    /// <returns>the language name id</returns>
    public string GetDefaultLanguage()
        => _cache_default_lang;
}