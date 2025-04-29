using ETL.Domain.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.Targets.DbTargets
{
    public class TargetTableConfig
    {
        [JsonPropertyName("TargetTable")]
        public string TargetTable { get; set; }
        [JsonPropertyName("Fields")]
        public List<LoadFieldMapRule> Fields { get; set; } = new();
    }
}
