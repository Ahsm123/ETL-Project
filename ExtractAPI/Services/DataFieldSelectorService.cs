using System.Text.Json;

namespace ExtractAPI.Services;

public class DataFieldSelectorService
{
    public IEnumerable<Dictionary<string, object>> FilterFields(JsonElement data, List<string> fields)
    {
        if(fields == null || fields.Count == 0)
        {
            return Enumerable.Empty<Dictionary<string, object>>();
        }

        var result = new List<Dictionary<string, object>>();

        foreach (var item in data.EnumerateArray())
        {
            var filteredItem = new Dictionary<string, object>();

            foreach (var field in fields)
            {
                if (item.TryGetProperty(field, out var value))
                {
                    filteredItem[field] = ConvertValue(value);
                }
            }

            result.Add(filteredItem);
        }
        return result;
    }

    private static object ConvertValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Number => value.GetDouble(),
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => value.ToString() ?? string.Empty
        };
    }
}
