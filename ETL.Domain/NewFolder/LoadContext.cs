using ETL.Domain.Targets.DbTargets;
using ETL.Domain.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.NewFolder
{
    public class LoadContext
    {
        public TargetInfoBase TargetInfo { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public string? PipelineId { get; set; }
        public List<TargetTableConfig>? Tables { get; set; }
    }
}
