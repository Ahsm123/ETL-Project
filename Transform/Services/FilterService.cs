using ETL.Domain.Rules;

namespace Transform.Services;

public class FilterService
{
    public bool ShouldInclude(Dictionary<string, object> item, List<FilterRule> filters)
    {
        foreach (var filter in filters)
        {
            if (!item.TryGetValue(filter.Field, out var rawValue))
            {
                return false;
            }

            var strValue = rawValue?.ToString() ?? string.Empty;

            if (!Evaluate(strValue, filter.Operator, filter.Value))
                return false;
        }
        return true;
    }

    private bool Evaluate(string fieldValue, string output, string expectedValue)
    {
        return output.ToLower() switch
        {
            "euqals" => fieldValue == expectedValue,
            "notequals" => fieldValue != expectedValue,
            "contains" => fieldValue.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
            "greaterthan" => double.TryParse(fieldValue, out var f1) && double.TryParse(expectedValue, out var f2) && f2 > f1,
            "lessthan" => double.TryParse(fieldValue, out var f3) && double.TryParse(expectedValue, out var f4) && f3 < f4,
        };
    }
}
