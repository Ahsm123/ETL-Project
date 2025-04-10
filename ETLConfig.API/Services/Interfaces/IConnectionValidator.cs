using ETLConfig.API.Models.DTOs;

namespace ETLConfig.API.Services.Interfaces;

public interface IConnectionValidator
{
    Task<bool> IsValidAsync(string connectionString);
    Task<DatabaseMetadata> GetMetadataAsync(string connectionString);
}
