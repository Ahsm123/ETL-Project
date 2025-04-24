using ETL.Domain.NewFolder;
using ETL.Domain.Rules;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using Load.Interfaces;

namespace Load.Writers;

public class MsSqlTargetWriter : ITargetWriter
{
    private readonly IMsSqlQueryBuilder _queryBuilder;
    private readonly IMsSqlExecutor _executor;

    public MsSqlTargetWriter(IMsSqlQueryBuilder queryBuilder, IMsSqlExecutor executor)
    {
        _queryBuilder = queryBuilder;
        _executor = executor;
    }

    public bool CanHandle(Type targetInfoType)
        => typeof(MsSqlTargetInfo).IsAssignableFrom(targetInfoType);

    public async Task WriteAsync(LoadContext context)
    {
        if (context.TargetInfo is not MsSqlTargetInfo info)
            throw new ArgumentException("Invalid target info type");

        if (info.UseBulkInsert)
            throw new NotImplementedException("Bulk insert is not implemented yet.");

        foreach (var table in context.Tables)
        {
            var mappedData = ApplyTargetMappings(context.Data, table.Fields);
            var (sql, parameters) = _queryBuilder.GenerateInsertQuery(table.TargetTable, mappedData);

            await _executor.ExecuteQueryAsync(info.ConnectionString, sql, parameters);
        }
    }

    private Dictionary<string, object> ApplyTargetMappings(Dictionary<string, object> data, List<LoadFieldMapRule> mappings)
    {
        if (mappings == null || mappings.Count == 0) return data;

        var mapped = new Dictionary<string, object>();
        foreach (var map in mappings)
        {
            if (data.TryGetValue(map.SourceField, out var value))
            {
                mapped[map.TargetField] = value;
            }
        }
        return mapped;
    }
}
