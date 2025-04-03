using ETL.Domain.Sources;
using ExtractAPI.DataSources;

namespace ExtractAPI.Factories;

public interface ISourceProviderFactory
{
    IDataSourceProvider GetProvider(SourceInfoBase sourceInfo);
}
