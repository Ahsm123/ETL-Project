using ETL.Domain.Rules;
using ETL.Domain.Sources;
using System.Text.Json;

namespace ExtractAPI.Interfaces;

public interface IDataSourceProvider
{
    Task<JsonElement> GetDataAsync(SourceInfoBase sourceInfo, List<FilterRule>? filters = null);
    bool CanHandle(Type sourceInfoType);
}

