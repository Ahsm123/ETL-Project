namespace Load.Interfaces;

public interface ITargetWriterResolver
{
    ITargetWriter? Resolve(Type targetInfoType, IServiceProvider services);
}

