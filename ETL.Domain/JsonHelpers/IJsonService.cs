using System.Text.Json;

namespace ETL.Domain.JsonHelpers;

public interface IJsonService
{
    T? Deserialize<T>(Stream stream);
    T? Deserialize<T>(string json);
    string Serialize<T>(T obj);
    JsonElement? Parse(string json);
}
