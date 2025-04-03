using ETL.Domain.Attributes;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Writers;

[TargetType("mssql")]
public class MsSqlTargetWriter : ITargetWriter
{
    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data)
    {
        if(targetInfo is not MsSqlTargetInfo sqlInfo)
        {
            throw new ArgumentException("Invalid target info type");
        }

        Console.WriteLine($"[DEBUG] Simulating write to table '{sqlInfo.TargetTable}'");
        Console.WriteLine($"         ConnectionString: {sqlInfo.ConnectionString}");
        Console.WriteLine($"         UseBulkInsert: {sqlInfo.UseBulkInsert}");
        Console.WriteLine($"         Data: {string.Join(", ", data.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");

        await Task.CompletedTask;
    }
}
