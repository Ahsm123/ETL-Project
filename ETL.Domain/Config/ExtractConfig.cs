using System.ComponentModel.DataAnnotations;

namespace ETL.Domain.Model;

public class ExtractConfig
{
    [Required(ErrorMessage = "Felter er påkrævet.")]
    [MinLength(1, ErrorMessage = "Data skal indeholde mindst ét element.")]
    public List<string> Fields { get; set; }
    public List<FilterCondition> Filters { get; set; }
}
