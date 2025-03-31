using Confluent.Kafka;
using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Transform.Strategy
{
    public class MappingStrategy
    {
        private readonly List<FieldMapping> _mappings;

        public MappingStrategy(List<FieldMapping> mappings)
        {
            _mappings = mappings;
        }
        public Dictionary<string, object?> ApplyFieldMapping(JsonElement item, List<FieldMapping> mappings)
        {
            var result = new Dictionary<string, object?>();
            var mappedFields = new HashSet<string>(mappings.Select(m => m.SourceField));

            // Tilføj alle felter uændret først
            foreach (var property in item.EnumerateObject())
            {
                result[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.Number => property.Value.GetDouble(),
                    JsonValueKind.String => property.Value.GetString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => property.Value.ToString()
                };
            }

            // Overskriv med mappede felter
            foreach (var mapping in mappings)
            {
                if (item.TryGetProperty(mapping.SourceField, out var value))
                {
                    result.Remove(mapping.SourceField); // Fjern originalt navn
                    result[mapping.TargetField] = value.ValueKind switch
                    {
                        JsonValueKind.Number => value.GetDouble(),
                        JsonValueKind.String => value.GetString(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => value.ToString()
                    };
                }
            }

            return result;
        }
    }
}







