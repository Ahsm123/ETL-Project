using ETL.Domain.MetaDataModels;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;

namespace ETL.Domain.NewFolder;

public class LoadContext
{
    public TargetInfoBase TargetInfo { get; set; }
    public Dictionary<string, object> Data { get; set; }
    public string? PipelineId { get; set; }
    public List<TargetTableConfig>? Tables { get; set; }
    public DatabaseMetaData? DatabaseMetadata { get; set; }
}
