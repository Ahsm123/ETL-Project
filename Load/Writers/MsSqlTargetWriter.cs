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

    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null)
    {
        if (targetInfo is not MsSqlTargetInfo info)
            throw new ArgumentException("Invalid target info type");

        if (info.UseBulkInsert)
        {
            // Placeholder
            throw new NotImplementedException("Bulk insert is not implemented yet.");
        }

        var (sql, parameters) = _queryBuilder.GenerateInsertQuery(info, data);

        try
        {
            await _executor.ExecuteQueryAsync(info.ConnectionString, sql, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to write to MSSQL target: {ex.Message}", ex);
        }
    }
}
