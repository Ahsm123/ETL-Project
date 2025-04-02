using ETL.Domain.Model.DTOs;

namespace Transform.Services;

public interface ITransformPipeline
{
    ProcessedPayload Execute(ExtractedPayload input);
}

