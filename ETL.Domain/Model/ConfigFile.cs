using ETL.Domain.Model.SourceInfo;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETL.Domain.Model;

public class ConfigFile
{
    public string Id { get; set; }
    [Required(ErrorMessage = "Source er påkrævet.")]
    public string SourceType { get; set; }
    public SourceInfoBase SourceInfo { get; set; }
    public ExtractSettings Extract { get; set; }
    public TransformSettings Transform { get; set; }
    public LoadSettings Load { get; set; }

    public JsonElement Data { get; set; }

}
