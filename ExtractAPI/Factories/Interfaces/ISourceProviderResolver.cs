using ETL.Domain.Sources;
using ExtractAPI.DataSources.Interfaces;

namespace ExtractAPI.Factories.Interfaces;

public interface ISourceProviderResolver
{
    IDataSourceProvider? Resolve(Type sourceInfoType);
}

