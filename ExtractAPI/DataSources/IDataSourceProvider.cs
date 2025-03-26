using ETL.Domain.Model.SourceInfo;
using System.Text.Json;

namespace ExtractAPI.DataSources;

public interface IDataSourceProvider
{
    Task<JsonElement> GetDataAsync(SourceInfoBase sourceInfo);
}
