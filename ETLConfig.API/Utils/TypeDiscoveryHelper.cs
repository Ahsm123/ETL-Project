using System.Text.Json.Serialization;

namespace ETLConfig.API.Utils;

public class TypeDiscoveryHelper
{
    public static IEnumerable<object> GetJsonDerivedTypes(Type baseType)
    {
        var derivedAttributes = baseType
            .GetCustomAttributes(typeof(JsonDerivedTypeAttribute), false)
            .Cast<JsonDerivedTypeAttribute>();

        return derivedAttributes.Select(attr => new
        {
            TypeName = attr.DerivedType.Name,
            Discriminator = attr.TypeDiscriminator?.ToString() ?? "(null)"
        });
    }
}
