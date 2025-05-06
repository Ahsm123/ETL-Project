using Load.TargetWriters.Interfaces;

namespace Load.Services.Interfaces;

public interface ITargetWriterResolver
{
    ITargetWriter? Resolve(Type targetInfoType, IServiceProvider services);
}

