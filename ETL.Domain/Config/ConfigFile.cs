using ETL.Domain.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ETL.Domain.Config;

public class ConfigFile
{
    [Required]
    [JsonPropertyName("Id")]
    public string Id { get; set; }
    [Required]
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [Required]
    [JsonPropertyName("Description")]
    public string Description { get; set; }
    [Required]
    [JsonPropertyName("Version")]
    public string Version { get; set; }

    [Required]
    [JsonPropertyName("ExtractConfig")]
    public ExtractConfig ExtractConfig { get; set; }
    [Required]
    [JsonPropertyName("TransformConfig")]
    public TransformConfig TransformConfig { get; set; }
    [Required]
    [JsonPropertyName("LoadTargetConfig")]
    public LoadTargetConfig LoadTargetConfig { get; set; }
}
