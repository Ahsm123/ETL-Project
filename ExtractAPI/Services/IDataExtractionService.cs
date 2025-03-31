using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using System.Text.Json;

namespace ExtractAPI.Services;

public interface IDataExtractionService
{
    Task<ExtractResponseDto> ExtractAsync(string configId);
}
