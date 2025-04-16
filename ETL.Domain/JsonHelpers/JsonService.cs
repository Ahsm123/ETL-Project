using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ETL.Domain.JsonHelpers;

public class JsonService : IJsonService
{
    private readonly JsonSerializerOptions _options;

    public JsonService(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                new DefaultJsonTypeInfoResolver())
        };
    }

    public T? Deserialize<T>(Stream stream)
        => JsonSerializer.Deserialize<T>(stream, _options);

    public T? Deserialize<T>(string json)
        => JsonSerializer.Deserialize<T>(json, _options);

    public string Serialize<T>(T obj)
        => JsonSerializer.Serialize(obj, _options);

    public JsonElement? Parse(string json)
        => JsonDocument.Parse(json).RootElement.Clone();
}