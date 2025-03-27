using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model;

public class ExtractSettings
{
    [Required(ErrorMessage = "Felter er påkrævet.")]
    [MinLength(1, ErrorMessage = "Data skal indeholde mindst ét element.")]
    public List<string> Fields { get; set; }
    public List<FilterCondition> Filters { get; set; }
}
