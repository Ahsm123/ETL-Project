using ETL.Domain.Model.SourceInfo;
using System.ComponentModel.DataAnnotations;

namespace ETL.Domain.Model
{
    public class ConfigFile
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Source er påkrævet.")]
        public string SourceType { get; set; }
        [Required(ErrorMessage = "SourceInfo er påkrævet.")]
        public SourceInfoBase SourceInfo {get; set; }
        public ExtractSettings Extract { get; set; }
        public TransformSettings Transform { get; set; }
        public LoadSettings Load { get; set; }

    }
}
