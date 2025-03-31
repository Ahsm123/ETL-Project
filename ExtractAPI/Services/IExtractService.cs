using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using System.Text.Json;

namespace ExtractAPI.Services;

public interface IExtractService
{
    Task<ExtractResponseDto> ExtractAsync(string configId);
}
