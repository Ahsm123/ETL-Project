using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using Load.Writers.Interfaces;

namespace Load.Writers;

public class MsSqlTargetWriter : ITargetWriter
{
    public bool CanHandle(Type targetInfoType)
        => typeof(MsSqlTargetInfo).IsAssignableFrom(targetInfoType);

    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data)
    {
        if (targetInfo is not MsSqlTargetInfo sqlInfo)
            throw new ArgumentException("Invalid target info type");

        Console.WriteLine($"[SQL] Writing to {sqlInfo.TargetTable} with bulk insert = {sqlInfo.UseBulkInsert}");
        await Task.CompletedTask;
    }
}

