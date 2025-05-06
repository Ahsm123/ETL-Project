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
        //[JsonPropertyName("LoadMode")]
        //public string LoadMode { get; set; }
        //[JsonPropertyName("TargetTables")]
        //public List<TargetTableConfig> TargetTables { get; set; } = new();
        public MySqlTargetInfo()
        {
        }
    }
}
