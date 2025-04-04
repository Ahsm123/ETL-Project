using ETL.Domain.Sources;
using ExtractAPI.DataSources;

namespace ExtractAPI.Factories;

public interface ISourceProviderResolver
{
    IDataSourceProvider? Resolve(Type sourceInfoType);
}

