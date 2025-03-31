using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Transform.Kafka;
using Transform.Strategy;

namespace Transform.Services
{
    public class TransformService : ITransformService<string>
    {
        private readonly MappingStrategy _mappingStrategy;

        public TransformService(MappingStrategy mappingStrategy)
        {
            _mappingStrategy = mappingStrategy;
        }

        public Task<string> TransformDataAsync(ExtractedPayload input)
        {
            var mappings = input.Transform?.Mappings ?? new List<FieldMapping>();

            // Map data
            JsonElement transformedData;

            if (input.Data.ValueKind == JsonValueKind.Array)
            {
                var items = input.Data.EnumerateArray().Select(item =>
                    _mappingStrategy.ApplyFieldMapping(item, mappings)).ToList();

                transformedData = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(items));
            }
            else
            {
                var item = _mappingStrategy.ApplyFieldMapping(input.Data, mappings);
                transformedData = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(item));
            }

            var payload = new TransformPayload
            {
                Id = input.Id,
                SourceType = input.SourceType,
                Load = input.Load,
                Data = transformedData
            };
            var json = JsonSerializer.Serialize(payload);
            return Task.FromResult(json);


        }

    }
}
    


