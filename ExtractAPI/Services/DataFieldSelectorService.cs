using ETL.Domain.Events;
using System.Text.Json;

namespace ExtractAPI.Services;

public class DataFieldSelectorService
{
    public IEnumerable<RawRecord> FilterFields(JsonElement data, List<string> fields)
    {
        if (fields == null || fields.Count == 0)
            return Enumerable.Empty<RawRecord>();

        return ExtractFields(data, fields);
    }

    private IEnumerable<RawRecord> ExtractFields(JsonElement data, List<string> fields)
    {
        foreach (var item in data.EnumerateArray())
        {
            var dict = new Dictionary<string, object>();

            foreach (var field in fields)
            {
                if (item.TryGetProperty(field, out var value))
                {
                    dict[field] = ConvertValue(value);
                }
            }

            yield return new RawRecord(dict);
        }
    }

    private static object ConvertValue(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.Number => value.TryGetInt64(out var i) ? i : value.GetDouble(),
        JsonValueKind.String => value.GetString() ?? "",
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        _ => value.ToString() ?? ""
    };
}

