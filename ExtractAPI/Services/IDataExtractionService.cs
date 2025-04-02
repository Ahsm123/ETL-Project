using ETL.Domain.Model.DTOs;

namespace ExtractAPI.Services;
public interface IDataExtractionService
{
    Task<ExtractResponseDto> ExtractAsync(string configId);
}
