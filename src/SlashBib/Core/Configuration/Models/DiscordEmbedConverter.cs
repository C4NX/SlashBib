using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlashBib.Core.Configuration.Models;

public class DiscordEmbedConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.StartObject:
                var obj = JObject.Load(reader);

                var embedBuilder = new DiscordEmbedBuilder();
                
                var colorToken = obj["color"];
                if (colorToken != null)
                    embedBuilder.WithColor(new DiscordColor(colorToken.Value<string>()?.Replace("#", null)));
                
                var descriptionToken = obj["description"];
                if (descriptionToken != null)
                    embedBuilder.WithDescription(descriptionToken.Value<string>());

                return embedBuilder.Build();
                break;
            default:
                return null;
        }
    }

    public override bool CanConvert(Type objectType)
        => objectType.IsAssignableTo(typeof(DiscordEmbed));
}