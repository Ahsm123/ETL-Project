using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.Rules;

public class LoadFieldMapRule
{
    [JsonPropertyName("SourceField")]
    public string SourceField { get; set; }

    [JsonPropertyName("TargetField")]
    public string TargetField { get; set; }
}
