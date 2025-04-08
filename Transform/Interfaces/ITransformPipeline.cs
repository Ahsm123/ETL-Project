using ETL.Domain.Events;

namespace Transform.Interfaces;

public interface ITransformPipeline
{
    TransformedEvent? Run(ExtractedEvent input);
}

