namespace ETLConfig.API.Models.DTOs;

public class DatabaseMetadata
{
    public List<TableMetadata> Tables { get; set; } = new();
}
