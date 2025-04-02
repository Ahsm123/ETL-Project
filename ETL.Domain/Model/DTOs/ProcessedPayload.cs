using ETL.Domain.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.DTOs;

public class ProcessedPayload
{
    public string PipelineId { get; set; }
    public string SourceType { get; set; }
    public LoadTargetConfig Load { get; set; }
    public Dictionary<string, object> Data { get; set; }
}
