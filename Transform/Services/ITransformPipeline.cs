using ETL.Domain.Events;

namespace Transform.Services;

public interface ITransformPipeline
{
    TransformedEvent Execute(ExtractedEvent input);
}

