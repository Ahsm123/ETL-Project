using ETL.Domain.Targets;

namespace Load.Interfaces;

public interface ITargetWriter
{
    bool CanHandle(Type targetInfoType);
    Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null);
}
