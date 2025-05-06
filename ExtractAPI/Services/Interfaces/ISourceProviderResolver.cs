using ETL.Domain.Sources;
using ExtractAPI.SourceProviders.Interfaces;

namespace ExtractAPI.Services.Interfaces;

public interface ISourceProviderResolver
{
    IDataSourceProvider? Resolve(Type sourceInfoType);
}

