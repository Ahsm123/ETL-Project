namespace ETLConfig.API.Models.DTOs;

public class ForeignKeyInfo
{
    public string Column { get; set; }
    public string ReferencedTable { get; set; }
    public string ReferencedColumn { get; set; }
}
