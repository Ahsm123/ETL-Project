using ETL.Domain.Sources;

namespace ExtractAPI.Interfaces;

public interface ISourceProviderResolver
{
    IDataSourceProvider? Resolve(Type sourceInfoType);
}

