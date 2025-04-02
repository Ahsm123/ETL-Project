using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class FileSourceBaseInfo : SourceInfoBase
{
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; }
    [JsonPropertyName("fileType")]
    public string FileType { get; set; }
}
