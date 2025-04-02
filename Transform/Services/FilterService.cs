using ETL.Domain.Model;
using System.Text.Json;

namespace Transform.Services;

public class FilterService
{
    public bool ShouldInclude(Dictionary<string, object> item, List<FilterCondition> filters)
    {
        foreach (var filter in filters)
        {
            if (!item.TryGetValue(filter.Field, out var rawValue))
                return false;

            if (!Evaluate(rawValue, filter))
                return false;
        }

        return true;
    }

    private bool Evaluate(object fieldValue, FilterCondition filter)
    {
        var op = filter.Operator.ToLower();
        var expectedValue = filter.Value;

        // numeric case (finally correct)
        if (TryGetDouble(fieldValue, out var actualNum) && double.TryParse(expectedValue, out var expectedNum))
        {
            return op switch
            {
                "equals" => actualNum == expectedNum,
                "greaterthan" or "greater_than" => actualNum > expectedNum,
                "lessthan" or "less_than" => actualNum < expectedNum,
                _ => false
            };
        }

        // fallback string case (JsonElement safe)
        var actualString = fieldValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String
            ? jsonElement.GetString() ?? string.Empty
            : fieldValue?.ToString() ?? string.Empty;

        return op switch
        {
            "equals" => actualString == expectedValue,
            "notequals" => actualString != expectedValue,
            "contains" => actualString.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }


    private bool TryGetDouble(object value, out double result)
    {
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Number)
            {
                return json.TryGetDouble(out result);
            }
            if (json.ValueKind == JsonValueKind.String)
            {
                return double.TryParse(json.GetString(), out result);
            }
        }
        else if (value is double d) { result = d; return true; }
        else if (value is int i) { result = i; return true; }
        else if (value is long l) { result = l; return true; }
        else if (value is float f) { result = f; return true; }
        else if (value is decimal dec) { result = (double)dec; return true; }
        else if (value is string s && double.TryParse(s, out var parsed)) { result = parsed; return true; }

        result = 0;
        return false;
    }
}

