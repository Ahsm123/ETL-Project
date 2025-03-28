using System.Text.Json;

namespace ExtractAPI.Services;

public class FieldFilterService
{
    // Filtrer dataen baseret på de properties, der er specificeret i config
    public IEnumerable<Dictionary<string, object>> FilterFields(JsonElement data, List<string> fields)
    {
        var result = new List<Dictionary<string, object>>();

        foreach (var item in data.EnumerateArray())
        {
            var filteredItem = new Dictionary<string, object>();

            foreach (var field in fields)
            {
                if (item.TryGetProperty(field, out var value))
                {
                    filteredItem[field] = value.ValueKind switch
                    {
                        JsonValueKind.Number => value.GetDouble(),
                        JsonValueKind.String => value.GetString() ?? string.Empty,
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => value.ToString() ?? string.Empty
                    };
                }
            }

            result.Add(filteredItem);
        }
        return result;
    }
}
