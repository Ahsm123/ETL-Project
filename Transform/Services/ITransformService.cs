using ETL.Domain.Model.DTOs;

namespace Transform.Services
{
    public interface ITransformService<T>
    {
        Task<T> TransformDataAsync(ExtractedPayload input);
    }
}
