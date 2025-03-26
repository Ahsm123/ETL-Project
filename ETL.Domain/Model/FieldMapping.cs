using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model;

public class FieldMapping
{
    public string SourceField { get; set; }
    public string TargetField { get; set; }
}
