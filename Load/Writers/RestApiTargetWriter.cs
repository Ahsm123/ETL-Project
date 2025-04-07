using ETL.Domain.Targets.ApiTargets;
using ETL.Domain.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Load.Writers.Interfaces;

namespace Load.Writers;

public class RestApiTargetWriter : ITargetWriter
{
    public bool CanHandle(Type targetInfoType)
        => typeof(RestApiTargetInfo).IsAssignableFrom(targetInfoType);

    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data)
    {
        if (targetInfo is not RestApiTargetInfo apiInfo)
            throw new ArgumentException("Invalid target info type");

        Console.WriteLine($"[API] Sending {apiInfo.Method} to {apiInfo.Url}");
        await Task.CompletedTask;
    }
}

