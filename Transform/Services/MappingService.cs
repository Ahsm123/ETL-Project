using ETL.Domain.Rules;

namespace Transform.Services;

// MappingService.cs
public class MappingService
{
    public Dictionary<string, object?> Apply(Dictionary<string, object> item, List<FieldMapRule> mappings)
    {
        var result = new Dictionary<string, object?>(item);

        foreach (var mapping in mappings)
        {
            if (item.TryGetValue(mapping.SourceField, out var value))
            {
                result.Remove(mapping.SourceField);
                result[mapping.TargetField] = value;
            }
        }

        return result;
    }
}

