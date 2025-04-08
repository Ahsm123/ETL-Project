using ETL.Domain.Events;

namespace Transform.Interfaces
{
    public interface ITransformService<T>
    {
        Task<T> TransformDataAsync(ExtractedEvent input);
    }
}
