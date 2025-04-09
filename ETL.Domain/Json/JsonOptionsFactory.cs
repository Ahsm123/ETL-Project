using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ETL.Domain.Json;

public static class JsonOptionsFactory
{
    public static JsonSerializerOptions Default => new()
    {
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = JsonTypeInfoResolver.Combine(
            new DefaultJsonTypeInfoResolver())
    };
}
