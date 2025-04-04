namespace Load.Writers;

public interface ITargetWriterResolver
{
    ITargetWriter? Resolve(Type targetInfoType, IServiceProvider services);
}

