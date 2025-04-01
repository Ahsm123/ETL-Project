using ETL.Domain.Model.DTOs;

namespace Transform.Services;

public interface ITransformPipeline
{
    TransformPayload Execute(ExtractedPayload input);
}

