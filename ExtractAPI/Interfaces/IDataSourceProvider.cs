using ETL.Domain.Model;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using System.Text.Json;

namespace ExtractAPI.Interfaces;

public interface IDataSourceProvider
{
    Task<JsonElement> GetDataAsync(ExtractConfig extractConfig);
    bool CanHandle(Type sourceInfoType);
}

