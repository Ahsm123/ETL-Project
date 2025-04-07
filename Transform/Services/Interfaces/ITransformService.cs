using ETL.Domain.Events;

namespace Transform.Services.Interfaces
{
    public interface ITransformService<T>
    {
        Task<T> TransformDataAsync(ExtractedEvent input);
    }
}
