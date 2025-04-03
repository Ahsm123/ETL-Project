using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class FileSourceBaseInfo : SourceInfoBase
{
    [JsonPropertyName("FilePath")]
    public string FilePath { get; set; }

    [JsonPropertyName("FileType")]
    public string FileType { get; set; }
}
