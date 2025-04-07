namespace Load.Writers.Interfaces;

public interface ITargetWriterResolver
{
    ITargetWriter? Resolve(Type targetInfoType, IServiceProvider services);
}

