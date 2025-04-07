using ETL.Domain.Sources;
using System.Text.Json;

namespace ExtractAPI.DataSources.Interfaces;

public interface IDataSourceProvider
{
    Task<JsonElement> GetDataAsync(SourceInfoBase sourceInfo);
    bool CanHandle(Type sourceInfoType);
}

