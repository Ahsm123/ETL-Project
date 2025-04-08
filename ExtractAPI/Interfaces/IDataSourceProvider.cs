using ETL.Domain.Sources;
using System.Text.Json;

namespace ExtractAPI.Interfaces;

public interface IDataSourceProvider
{
    Task<JsonElement> GetDataAsync(SourceInfoBase sourceInfo);
    bool CanHandle(Type sourceInfoType);
}

