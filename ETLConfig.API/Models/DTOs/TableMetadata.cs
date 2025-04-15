namespace ETLConfig.API.Models.DTOs;

public class TableMetadata
{
    public string TableName { get; set; }
    public List<ColumnMetadata> Columns { get; set; } = new();
    public List<string> PrimaryKeys { get; set; } = new();
    public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();
}
