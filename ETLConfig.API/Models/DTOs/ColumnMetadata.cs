namespace ETLConfig.API.Models.DTOs;
public class ColumnMetadata
{
    public string ColumnName { get; set; }
    public string DataType { get; set; }
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
}
