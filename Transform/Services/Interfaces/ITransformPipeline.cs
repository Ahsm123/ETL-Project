using ETL.Domain.Events;

namespace Transform.Services.Interfaces;

public interface ITransformPipeline
{
    TransformedEvent? Execute(ExtractedEvent input);
}

