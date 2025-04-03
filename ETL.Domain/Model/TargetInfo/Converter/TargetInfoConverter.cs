using ETL.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETL.Domain.Model.TargetInfo.Converter
{
    public class TargetInfoConverter : JsonConverter<TargetInfoBase>
    {
        public override TargetInfoBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);

            if (!doc.RootElement.TryGetProperty("Type", out var typeProp))
                throw new JsonException("Missing 'Type' discriminator in TargetInfo.");

            var typeName = typeProp.GetString();
            if (typeName == null)
                throw new JsonException("Null 'Type' value in TargetInfo.");

            var targetType = TargetTypeMapper.GetTargetInfoType(typeName);
            if (targetType == null)
                throw new JsonException($"Unknown TargetInfo type: '{typeName}'");

            return (TargetInfoBase)JsonSerializer.Deserialize(doc.RootElement.GetRawText(), targetType, options)!;
        }

        public override void Write(Utf8JsonWriter writer, TargetInfoBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
