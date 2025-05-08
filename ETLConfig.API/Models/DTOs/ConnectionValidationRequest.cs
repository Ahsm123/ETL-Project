using System.ComponentModel.DataAnnotations;

namespace ETLConfig.API.Models.DTOs;

public class ConnectionValidationRequest
{
    [Required]
    public required string ConnectionString { get; set; }
    [Required]
    public required string Type { get; set; }
}
