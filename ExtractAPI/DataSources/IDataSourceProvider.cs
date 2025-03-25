using System.Text.Json;

namespace ExtractAPI.DataSources;

public interface IDataSourceProvider
{
    Task<JsonElement> GetDataAsync(string sourceInfo);
}
