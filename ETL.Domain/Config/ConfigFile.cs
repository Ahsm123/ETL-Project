using ETL.Domain.Model;
using ETL.Domain.Model.SourceInfo;
using System.ComponentModel.DataAnnotations;

namespace ETL.Domain.Config;

public class ConfigFile
{
    public string Id { get; set; }
    [Required(ErrorMessage = "Source er påkrævet.")]
    public string SourceType { get; set; }
    public SourceInfoBase SourceInfo { get; set; }
    public ExtractConfig Extract { get; set; }
    public TransformConfig Transform { get; set; }
    public LoadTargetConfig Load { get; set; }


}
