using ETL.Domain.Attributes;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

[SourceType("excel")]
public class ExcelSourceInfo : FileSourceBaseInfo
{
    [JsonPropertyName("sheetName")]
    public string SheetName { get; set; } = "TestSheet";
}
