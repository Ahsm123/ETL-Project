using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.Targets.DbTargets
{
    public class MySqlTargetInfo : DbTargetInfoBase
    {
        [JsonPropertyName("UseSsl")]
        public bool UseSsl { get; set; }
        public MySqlTargetInfo()
        {
        }
    }
    
    
}
