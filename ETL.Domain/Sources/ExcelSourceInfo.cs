using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class ExcelSourceInfo : FileSourceBaseInfo
{
    [JsonPropertyName("SheetName")]
    public string SheetName { get; set; } = "TestSheet";
}
