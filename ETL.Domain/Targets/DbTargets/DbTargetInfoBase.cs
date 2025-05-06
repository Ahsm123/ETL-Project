using ETL.Domain.Rules;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

public abstract class DbTargetInfoBase : TargetInfoBase
{
    [Required]
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; }
    [JsonPropertyName("LoadMode")]
    public string LoadMode { get; set; }


    protected DbTargetInfoBase()
    {
    }
}
