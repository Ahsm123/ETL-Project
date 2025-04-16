using ETL.Domain.Events;
using ETL.Domain.Rules;

namespace Transform.Services;

public class MappingService
{
    public RawRecord Apply(RawRecord input, List<FieldMapRule> mappings)
    {
        var result = new Dictionary<string, object?>(input.Fields);

        foreach (var mapping in mappings)
        {
            if (input.Fields.TryGetValue(mapping.SourceField, out var value))
            {
                result.Remove(mapping.SourceField);
                result[mapping.TargetField] = value;
            }
        }

        return new RawRecord(result);
    }
}
