using ETL.Domain.NewFolder;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using System.Runtime.Loader;

namespace Load.TargetWriters.Interfaces;

public interface ITargetWriter
{
    bool CanHandle(Type targetInfoType);
    Task WriteAsync(LoadContext context);
}
