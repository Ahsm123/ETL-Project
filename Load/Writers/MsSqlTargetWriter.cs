using ETL.Domain.Attributes;
using ETL.Domain.Model.TargetInfo;
using ETL.Domain.Model.TargetInfo.DbTargets;
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

        Console.WriteLine($"Writing to MSSQL: {sqlInfo.TableName} | Data: {data.Count} fields");
        await Task.CompletedTask;
    }
}
