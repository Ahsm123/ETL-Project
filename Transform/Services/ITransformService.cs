using ETL.Domain.Events;

namespace Transform.Services
{
    public interface ITransformService<T>
    {
        Task<T> TransformDataAsync(ExtractedEvent input);
    }
}
