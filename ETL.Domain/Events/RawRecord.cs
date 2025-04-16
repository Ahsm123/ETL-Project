using System.Text.Json;

namespace ETL.Domain.Events;

public record RawRecord(Dictionary<string, object> Fields)
{
    public Dictionary<string, object> GetNormalized()
    {
        return Fields.ToDictionary(
            kvp => kvp.Key,
            kvp => NormalizeValue(kvp.Value)
        );
    }

    private static object NormalizeValue(object value)
    {
        return value switch
        {
            JsonElement json => json.ValueKind switch
            {
                JsonValueKind.String => json.GetString() ?? "",
                JsonValueKind.Number => json.TryGetDecimal(out var d) ? d : json.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => DBNull.Value,
                _ => json.ToString() ?? ""
            },
            _ => value
        };
    }
}



